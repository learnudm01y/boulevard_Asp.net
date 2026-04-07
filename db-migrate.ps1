<#
.SYNOPSIS
    Boulevard - Apply new DB changes to the database

.DESCRIPTION
    Applies all recent database changes:
      - IcvBoulevardScore column on Products and TempProducts
      - CommissionRate column on FeatureCategories
      - Commission rate seed data for all 12 portals
      - Performance indexes on main tables

    Usage:
      powershell -ExecutionPolicy Bypass -File db-migrate.ps1 -DryRun
      powershell -ExecutionPolicy Bypass -File db-migrate.ps1
      powershell -ExecutionPolicy Bypass -File db-migrate.ps1 -Server "MYSERVER\SQL2019" -Database "BoulevardDb"
      powershell -ExecutionPolicy Bypass -File db-migrate.ps1 -ForceCommissionRates
#>

[CmdletBinding()]
param(
    [switch]$DryRun,
    [string]$Server   = "",
    [string]$Database = "",
    [string]$Username = "",
    [string]$Password = "",
    [switch]$ForceCommissionRates,
    [switch]$SkipMigrations,
    [switch]$SkipIndexes
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$WEB_ROOT = "C:\inetpub\wwwroot\boulevard.r-y-x.net"

# ── Auto-detect connection string from Web.config on the server ──────────────
# This way the script always uses the exact same connection the app uses,
# regardless of whether it runs on the web server or the DB server itself.
$webConfigPath = Join-Path $WEB_ROOT "Web.config"
if ((-not $webConfigPath) -or (-not (Test-Path $webConfigPath))) {
    # fall back: look next to the script itself
    $webConfigPath = Join-Path $PSScriptRoot "Web.config"
}

if ((Test-Path $webConfigPath) -and ($Server -eq "" -or $Database -eq "")) {
    try {
        [xml]$wc = Get-Content $webConfigPath -Raw -Encoding UTF8
        $cs = ($wc.configuration.connectionStrings.add | Where-Object { $_.name -eq "BoulevardDbContext" }).connectionString
        if ($cs) {
            # Parse individual keys (case-insensitive, handles spaces around =)
            function Parse-CSKey { param([string]$cs, [string]$key)
                if ($cs -match "(?i)(?:^|;)\s*$key\s*=\s*([^;]+)") { return $Matches[1].Trim() }
                return ""
            }
            if ($Server   -eq "") { $Server   = Parse-CSKey $cs "Data Source" }
            if ($Database -eq "") { $Database = Parse-CSKey $cs "Initial Catalog" }
            if ($Username -eq "") { $Username = Parse-CSKey $cs "user id" }
            if ($Password -eq "") { $Password = Parse-CSKey $cs "password" }
            # Integrated Security fallback
            $intSec = Parse-CSKey $cs "Integrated Security"
        }
    } catch { <# ignore parse errors, fall through to defaults #> }
}

# Final fallback defaults (only used if Web.config not found at all)
if ($Database -eq "") { $Database = "BoulevardDb" }
if ($Server   -eq "") {
    # No Web.config found and no -Server argument supplied.
    # We are NOT on a local dev machine — fail early with a clear message.
    Write-Host ""
    Write-Host "  [XX] FAILED: Cannot determine SQL Server." -ForegroundColor Red
    Write-Host "  Place this script next to Web.config, or supply:" -ForegroundColor Yellow
    Write-Host "    -Server 'EC2AMAZ-I1IPIU5\SQLEXPRESS' -Username 'BoulevardDb-user' -Password 'xxx'" -ForegroundColor Yellow
    exit 1
}

$MIGRATE_CANDIDATES = @(
    "$WEB_ROOT\bin\migrate.exe",
    "$PSScriptRoot\bin\migrate.exe",
    "$PSScriptRoot\packages\EntityFramework.6.5.1\tools\migrate.exe",
    "$PSScriptRoot\packages\EntityFramework.6.4.4\tools\migrate.exe",
    "$PSScriptRoot\packages\EntityFramework.6.2.0\tools\migrate.exe"
)

# Build ADO.NET connection string — uses .NET built into Windows, no sqlcmd install needed
if ($Username -ne "") {
    $ConnStr = "Data Source=$Server;Initial Catalog=$Database;User ID=$Username;Password=$Password;MultipleActiveResultSets=True;Connect Timeout=30;"
} else {
    $ConnStr = "Data Source=$Server;Initial Catalog=$Database;Integrated Security=True;MultipleActiveResultSets=True;Connect Timeout=30;"
}

function Invoke-SqlBatches { param([string]$Label, [string]$Sql)
    Log "[SQL] $Label"
    if ($DryRun) { Write-Info "[DRY] $Label"; return }
    $conn = New-Object System.Data.SqlClient.SqlConnection($ConnStr)
    try {
        $conn.Open()
        $batches = $Sql -split '(?im)^\s*GO\s*$' | Where-Object { $_.Trim() -ne "" }
        foreach ($batch in $batches) {
            $cmd = $conn.CreateCommand()
            $cmd.CommandText = $batch
            $cmd.CommandTimeout = 120
            $cmd.ExecuteNonQuery() | Out-Null
        }
        Write-OK $Label
    } catch {
        Log "SQL-ERR: $_"
        Exit-Fail "SQL failed: $Label `n$_"
    } finally {
        $conn.Close(); $conn.Dispose()
    }
}

function Invoke-SqlFile { param([string]$Label, [string]$FilePath)
    if (-not (Test-Path $FilePath)) { Write-Warn "File not found, skipping: $FilePath"; return }
    Log "SQL-FILE: $Label"
    if ($DryRun) { Write-Info "[DRY] $Label ($FilePath)"; return }
    $sql = [System.IO.File]::ReadAllText($FilePath, [System.Text.Encoding]::UTF8)
    Invoke-SqlBatches $Label $sql
}

function Invoke-SqlQuery { param([string]$Sql)
    $conn = New-Object System.Data.SqlClient.SqlConnection($ConnStr)
    $result = New-Object System.Text.StringBuilder
    try {
        $conn.Open()
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = $Sql
        $cmd.CommandTimeout = 30
        $reader = $cmd.ExecuteReader()
        while ($reader.Read()) {
            $row = @(); for ($i = 0; $i -lt $reader.FieldCount; $i++) { $row += "$($reader.GetName($i))=$($reader[$i])" }
            [void]$result.AppendLine(($row -join "  |  "))
        }
        $reader.Close()
    } finally { $conn.Close(); $conn.Dispose() }
    return $result.ToString()
}

# Log next to the script itself — works on any server without pre-creating folders
$LOG_DIR   = Join-Path $PSScriptRoot "logs"
$TIMESTAMP = Get-Date -Format 'yyyyMMdd_HHmmss'
$LOG_FILE  = Join-Path $LOG_DIR "db-migrate_$TIMESTAMP.log"

function Init-Log {
    New-Item -ItemType Directory -Path $LOG_DIR -Force -ErrorAction SilentlyContinue | Out-Null
    "[$TIMESTAMP] Boulevard DB Migration Started" | Out-File $LOG_FILE -Encoding UTF8 -ErrorAction SilentlyContinue
}

function Log { param([string]$m)
    try { "$(Get-Date -F 'HH:mm:ss') $m" | Add-Content $LOG_FILE -Encoding UTF8 -ErrorAction SilentlyContinue } catch {}
}

function Write-Step { param([string]$m)
    $b = "-" * 62
    Write-Host "`n$b"    -ForegroundColor Magenta
    Write-Host "  >> $m" -ForegroundColor Magenta
    Write-Host "$b"      -ForegroundColor Magenta
    Log "[STEP] $m"
}

function Write-OK   { param([string]$m) Write-Host "  [OK] $m" -ForegroundColor Green;  Log "[OK]   $m" }
function Write-Info { param([string]$m) Write-Host "  [..] $m" -ForegroundColor Cyan;   Log "[INFO] $m" }
function Write-Warn { param([string]$m) Write-Host "  [!!] $m" -ForegroundColor Yellow; Log "[WARN] $m" }

function Exit-Fail { param([string]$m)
    Write-Host "`n  [XX] FAILED: $m" -ForegroundColor Red
    Log "[ERROR] $m"
    exit 1
}

function Run-SQL { param([string]$Label, [string]$Sql)
    Log "[SQL] $Label"
    if ($DryRun) { Write-Info "[DRY] $Label"; return }
    $tmp = [System.IO.Path]::GetTempFileName() + ".sql"
    try {
        [System.IO.File]::WriteAllText($tmp, $Sql, [System.Text.Encoding]::UTF8)
        $out = & $SQLCMD @SqlArgs -i $tmp -b 2>&1
        if ($LASTEXITCODE -ne 0) {
            Log "SQL-ERR: $out"
            Exit-Fail "SQL failed: $Label `n$out"
        }
        Write-OK $Label
    } finally {
        Remove-Item $tmp -Force -ErrorAction SilentlyContinue
    }
}

function Run-SQLFile { param([string]$Label, [string]$FilePath)
    if (-not (Test-Path $FilePath)) { Write-Warn "File not found, skipping: $FilePath"; return }
    Log "SQL-FILE: $Label"
    if ($DryRun) { Write-Info "[DRY] $Label ($FilePath)"; return }
    $out = & $SQLCMD @SqlArgs -i $FilePath -b 2>&1
    if ($LASTEXITCODE -ne 0) { Log "SQL-ERR: $out"; Exit-Fail "SQL file failed: $Label `n$out" }
    Write-OK $Label
}

# ================================================================
Init-Log

Write-Host ""
Write-Host ("=" * 62) -ForegroundColor Cyan
Write-Host "   BOULEVARD - DB MIGRATION SCRIPT"           -ForegroundColor Cyan
Write-Host "   Server  : $Server"                         -ForegroundColor Cyan
Write-Host "   Database: $Database"                       -ForegroundColor Cyan
Write-Host "   User    : $Username"                       -ForegroundColor Cyan
Write-Host "   Date    : $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor Cyan
if ($DryRun) { Write-Host "   *** DRY RUN - no changes will be made ***" -ForegroundColor Yellow }
Write-Host ("=" * 62) -ForegroundColor Cyan

# ================================================================
# PHASE 0 - Pre-flight
# ================================================================
Write-Step "PHASE 0: Pre-flight Checks"

# Test DB connection using .NET — no sqlcmd required
try {
    $testConn = New-Object System.Data.SqlClient.SqlConnection($ConnStr)
    $testConn.Open()
    $testConn.Close(); $testConn.Dispose()
    Write-OK "DB connection OK: $Server / $Database"
} catch {
    Exit-Fail "Cannot connect to [$Server][$Database] --- $_"
}

$migrateTool = $null
foreach ($c in $MIGRATE_CANDIDATES) {
    if (Test-Path $c) { $migrateTool = $c; break }
}
if ($migrateTool) { Write-OK "migrate.exe found: $migrateTool" }
else { Write-Warn "migrate.exe not found - Phase 1 will be skipped"; $SkipMigrations = $true }

# ================================================================
# PHASE 1 - EF Migrations
# ================================================================
Write-Step "PHASE 1: EF Migrations"

if ($SkipMigrations) {
    Write-Warn "Skipping EF Migrations - run Update-Database from Visual Studio manually"
} else {
    $configFile = Join-Path $WEB_ROOT "bin\Boulevard.dll.config"
    if (-not (Test-Path $configFile)) {
        $configFile = Join-Path $PSScriptRoot "bin\Boulevard.dll.config"
    }
    if ($DryRun) {
        Write-Info "[DRY] Would run migrate.exe Boulevard.dll"
    } else {
        Write-Info "Running migrate.exe..."
        Push-Location (Split-Path $migrateTool -Parent)
        try {
            if (Test-Path $configFile) {
                $out = & $migrateTool "Boulevard.dll" "/startupConfigurationFile=$configFile" 2>&1
            } else {
                $out = & $migrateTool "Boulevard.dll" 2>&1
            }
            Log "MIGRATE: $out"
            if ($LASTEXITCODE -ne 0) { Exit-Fail "migrate.exe failed --- $out" }
            Write-OK "EF Migrations applied"
        } finally { Pop-Location }
    }
}

# ================================================================
# PHASE 2 - IcvBoulevardScore
# ================================================================
Write-Step "PHASE 2: IcvBoulevardScore column (Products & TempProducts)"

$sqlIcv = "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Products' AND COLUMN_NAME='IcvBoulevardScore') BEGIN ALTER TABLE dbo.Products ADD IcvBoulevardScore NVARCHAR(50) NULL; PRINT 'Added to Products.'; END ELSE PRINT 'Products.IcvBoulevardScore already exists.'; IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='TempProducts' AND COLUMN_NAME='IcvBoulevardScore') BEGIN ALTER TABLE dbo.TempProducts ADD IcvBoulevardScore NVARCHAR(50) NULL; PRINT 'Added to TempProducts.'; END ELSE PRINT 'TempProducts.IcvBoulevardScore already exists.';"

Invoke-SqlBatches "Add IcvBoulevardScore column" $sqlIcv

# ================================================================
# PHASE 3 - CommissionRate + seed data
# ================================================================
Write-Step "PHASE 3: CommissionRate column + 12 portal rates"

$sqlAddCol = "IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='FeatureCategories' AND COLUMN_NAME='CommissionRate') BEGIN ALTER TABLE dbo.FeatureCategories ADD CommissionRate DECIMAL(5,2) NULL; PRINT 'CommissionRate added.'; END ELSE PRINT 'CommissionRate already exists.';"

Invoke-SqlBatches "Add CommissionRate column" $sqlAddCol

if ($ForceCommissionRates) { $cond = "1=1" } else { $cond = "(CommissionRate IS NULL OR CommissionRate = 0)" }

$lines = @(
    "UPDATE dbo.FeatureCategories SET CommissionRate=3.00  WHERE FeatureCategoryKey='3b317e3f-cb2f-4fdd-b9c8-3f2186695771' AND $cond;",
    "UPDATE dbo.FeatureCategories SET CommissionRate=15.00 WHERE FeatureCategoryKey='E7B3A1C2-D4F5-4A6B-8C9D-1E2F3A4B5C6D' AND $cond;",
    "UPDATE dbo.FeatureCategories SET CommissionRate=7.00  WHERE FeatureCategoryKey='F1A2B3C4-D5E6-4F70-8B9C-0D1E2F3A4B5C' AND $cond;",
    "UPDATE dbo.FeatureCategories SET CommissionRate=15.00 WHERE FeatureCategoryKey='88d5d23e-470f-409a-bb6b-def7ab1346fa' AND $cond;",
    "UPDATE dbo.FeatureCategories SET CommissionRate=10.00 WHERE FeatureCategoryKey='f4309df5-9121-41ad-831a-994c46b62766' AND $cond;",
    "UPDATE dbo.FeatureCategories SET CommissionRate=5.00  WHERE FeatureCategoryKey='c286a46b-5b9a-4519-bb10-8d47ec254ffb' AND $cond;",
    "UPDATE dbo.FeatureCategories SET CommissionRate=15.00 WHERE FeatureCategoryKey='bbc98e2d-941b-44c6-8122-0e12a2645b87' AND $cond;",
    "UPDATE dbo.FeatureCategories SET CommissionRate=15.00 WHERE FeatureCategoryKey='25d8c418-2d26-4159-9d7f-970e3b933b42' AND $cond;",
    "UPDATE dbo.FeatureCategories SET CommissionRate=7.00  WHERE FeatureCategoryKey='b3e3e680-c8ef-4ab2-a4ac-d75bb48a3647' AND $cond;",
    "UPDATE dbo.FeatureCategories SET CommissionRate=5.00  WHERE FeatureCategoryKey='DD501B2D-FE22-4C31-B340-1B4237FAB5CC' AND $cond;",
    "UPDATE dbo.FeatureCategories SET CommissionRate=0.00  WHERE FeatureCategoryKey='3c4d5e6f-7a8b-9c0d-ef12-345678901abc' AND CommissionRate IS NULL;",
    "UPDATE dbo.FeatureCategories SET CommissionRate=0.00  WHERE FeatureCategoryKey='4d5e6f7a-8b9c-0d1e-f234-5678901abcde' AND CommissionRate IS NULL;",
    "PRINT 'Commission rates seeded.';"
)
$sqlRates = $lines -join "`r`n"

Invoke-SqlBatches "Seed commission rates (12 portals)" $sqlRates

# ================================================================
# PHASE 4 - Performance Indexes
# ================================================================
Write-Step "PHASE 4: Performance Indexes"

if ($SkipIndexes) {
    Write-Warn "Skipping indexes"
} else {
    # Look for the SQL file next to the script first, then in the web root
    $indexFile = Join-Path $PSScriptRoot "db_indexes_to_apply.sql"
    if (-not (Test-Path $indexFile)) { $indexFile = Join-Path $WEB_ROOT "db_indexes_to_apply.sql" }
    if (Test-Path $indexFile) {
        Invoke-SqlFile "Apply performance indexes from db_indexes_to_apply.sql" $indexFile
    } else {
        $idxLines = @(
            "IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_OrderRequestProducts_MemberId' AND object_id=OBJECT_ID('dbo.OrderRequestProducts')) CREATE INDEX IX_OrderRequestProducts_MemberId ON dbo.OrderRequestProducts (MemberId) INCLUDE (OrderStatusId, CreateDate);",
            "IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_OrderRequestProducts_IsSound' AND object_id=OBJECT_ID('dbo.OrderRequestProducts')) CREATE INDEX IX_OrderRequestProducts_IsSound ON dbo.OrderRequestProducts (IsSound) WHERE IsSound=0;",
            "IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_OrderRequestServices_MemberId' AND object_id=OBJECT_ID('dbo.OrderRequestServices')) CREATE INDEX IX_OrderRequestServices_MemberId ON dbo.OrderRequestServices (MemberId) INCLUDE (OrderStatusId, CreateDate);",
            "IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_OrderRequestServices_IsSound' AND object_id=OBJECT_ID('dbo.OrderRequestServices')) CREATE INDEX IX_OrderRequestServices_IsSound ON dbo.OrderRequestServices (IsSound) WHERE IsSound=0;",
            "IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_Members_PhoneNumber' AND object_id=OBJECT_ID('dbo.Members')) CREATE INDEX IX_Members_PhoneNumber ON dbo.Members (PhoneNumber) INCLUDE (FirstName, LastName, Email, Status);",
            "IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_Members_MemberKey' AND object_id=OBJECT_ID('dbo.Members')) CREATE UNIQUE INDEX IX_Members_MemberKey ON dbo.Members (MemberKey) WHERE MemberKey IS NOT NULL;",
            "IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_Products_Status_FeatureCategoryId' AND object_id=OBJECT_ID('dbo.Products')) CREATE INDEX IX_Products_Status_FeatureCategoryId ON dbo.Products (Status, FeatureCategoryId) INCLUDE (ProductName, Price, IcvBoulevardScore);",
            "PRINT 'Core indexes applied.';"
        )
        $sqlIdx = $idxLines -join "`r`n"
        Invoke-SqlBatches "Add core performance indexes" $sqlIdx
        Write-Warn "db_indexes_to_apply.sql not found - only core indexes applied"
    }
}

# ================================================================
# PHASE 5 - Verification
# ================================================================
Write-Step "PHASE 5: Verification"

if (-not $DryRun) {
    try {
        $vOut1 = Invoke-SqlQuery "SELECT TABLE_NAME, COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME='IcvBoulevardScore'"
        $vOut2 = Invoke-SqlQuery "SELECT Name, CommissionRate FROM dbo.FeatureCategories WHERE IsDelete=0 AND CommissionRate IS NOT NULL ORDER BY Name"
        $vOut3 = Invoke-SqlQuery "SELECT TOP 1 MigrationId FROM dbo.__MigrationHistory ORDER BY MigrationId DESC"
        Write-Host ""
        Write-Host "-- IcvBoulevardScore columns --" -ForegroundColor White
        Write-Host $vOut1 -ForegroundColor White
        Write-Host "-- Commission Rates --" -ForegroundColor White
        Write-Host $vOut2 -ForegroundColor White
        Write-Host "-- Last Migration --" -ForegroundColor White
        Write-Host $vOut3 -ForegroundColor White
        Log "VERIFY OK"
    } catch { Write-Warn "Verification query failed: $_" }
}

# ================================================================
Write-Host ""
Write-Host ("=" * 62) -ForegroundColor Green
if ($DryRun) {
    Write-Host "   DRY RUN complete - nothing was changed" -ForegroundColor Yellow
} else {
    Write-Host "   All changes applied successfully" -ForegroundColor Green
    Write-Host "   Open: https://boulevard.r-y-x.net/admin/social-impact-tracker" -ForegroundColor Cyan
}
Write-Host "   Log: $LOG_FILE" -ForegroundColor Gray
Write-Host ("=" * 62) -ForegroundColor Green