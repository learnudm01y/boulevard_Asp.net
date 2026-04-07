<#
.SYNOPSIS
    Boulevard - Apply new DB changes to the database

.DESCRIPTION
    Applies all recent database changes:
      - Origin column on Products and TempProducts (Social Impact)

    Usage:
      powershell -ExecutionPolicy Bypass -File db-migrate.ps1 -DryRun
      powershell -ExecutionPolicy Bypass -File db-migrate.ps1
      powershell -ExecutionPolicy Bypass -File db-migrate.ps1 -Server "MYSERVER\SQL2019" -Database "BoulevardDb"
      powershell -ExecutionPolicy Bypass -File db-migrate.ps1
#>

[CmdletBinding()]
param(
    [switch]$DryRun,
    [string]$Server   = "",
    [string]$Database = "",
    [string]$Username = "",
    [string]$Password = "",
    [switch]$SkipMigrations
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

# ================================================================
# PHASE 1 - EF Migrations
# ================================================================
Write-Step "PHASE 1: EF Migrations"

if ($SkipMigrations) {
    Write-Warn "Skipping EF Migrations - run Update-Database from Visual Studio manually"
} else {
    Write-Warn "migrate.exe not available - skipping EF migrations"
}

# ================================================================
# PHASE 2 - Origin (Social Impact)
# ================================================================
Write-Step "PHASE 2: Origin column (Products & TempProducts)"

$sqlOrigin = @"
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Products' AND COLUMN_NAME='Origin')
  BEGIN ALTER TABLE dbo.Products ADD Origin NVARCHAR(100) NULL; PRINT 'Added Origin to Products.'; END
ELSE PRINT 'Products.Origin already exists.';

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='TempProducts' AND COLUMN_NAME='Origin')
  BEGIN ALTER TABLE dbo.TempProducts ADD Origin NVARCHAR(100) NULL; PRINT 'Added Origin to TempProducts.'; END
ELSE PRINT 'TempProducts.Origin already exists.';
"@

Invoke-SqlBatches "Add Origin column" $sqlOrigin

# ================================================================
# PHASE 3 - Verification
# ================================================================
Write-Step "PHASE 3: Verification"

if (-not $DryRun) {
    try {
        $vOut1 = Invoke-SqlQuery "SELECT TABLE_NAME, COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME='Origin' ORDER BY TABLE_NAME"
        $vOut2 = Invoke-SqlQuery "SELECT TOP 1 MigrationId FROM dbo.__MigrationHistory ORDER BY MigrationId DESC"
        Write-Host ""
        Write-Host "-- Origin column --" -ForegroundColor White
        Write-Host $vOut1 -ForegroundColor White
        Write-Host "-- Last Migration --" -ForegroundColor White
        Write-Host $vOut2 -ForegroundColor White
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