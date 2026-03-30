namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class firstupdate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FeatureCategories",
                c => new
                    {
                        FeatureCategoryId = c.Int(nullable: false, identity: true),
                        FeatureCategoryKey = c.Guid(nullable: false),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.FeatureCategoryId);
            
            CreateTable(
                "dbo.LayoutSettings",
                c => new
                    {
                        layoutSetting = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 10),
                        LogoHeader = c.String(maxLength: 10),
                        MainHeader = c.String(maxLength: 10),
                        Body = c.String(maxLength: 10),
                        SideBar = c.String(maxLength: 10),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.layoutSetting);
            
            CreateTable(
                "dbo.Modules",
                c => new
                    {
                        ModuleId = c.Int(nullable: false, identity: true),
                        ModuleName = c.String(),
                        Status = c.String(maxLength: 10),
                        ParentId = c.Int(),
                        PositionId = c.Int(nullable: false),
                        UpdatedBy = c.Int(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                        ModuleCode = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.ModuleId);
            
            CreateTable(
                "dbo.RoleModules",
                c => new
                    {
                        RoleModuleId = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        UpdatedBy = c.Int(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RoleModuleId)
                .ForeignKey("dbo.Modules", t => t.ModuleId, cascadeDelete: true)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        RoleKey = c.Guid(nullable: false),
                        RoleName = c.String(maxLength: 200),
                        Status = c.String(maxLength: 10),
                        UpdatedBy = c.Int(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateTable(
                "dbo.UserActivities",
                c => new
                    {
                        UserActivityId = c.Long(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Details = c.String(maxLength: 200),
                        Action = c.String(maxLength: 100),
                        Url = c.String(maxLength: 300),
                        Ip = c.String(maxLength: 20),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserActivityId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserKey = c.Guid(nullable: false),
                        Name = c.String(maxLength: 250),
                        Email = c.String(maxLength: 250),
                        PhoneNumber = c.String(maxLength: 30),
                        Image = c.String(maxLength: 180),
                        RoleId = c.Int(nullable: false),
                        LayoutId = c.Int(),
                        UserName = c.String(maxLength: 100),
                        Password = c.String(),
                        OTP = c.String(maxLength: 255),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.WebHtmls",
                c => new
                    {
                        WebHtmlId = c.Long(nullable: false, identity: true),
                        WebHtmlkey = c.Guid(nullable: false),
                        Identifier = c.String(maxLength: 250),
                        PageIdentifier = c.String(maxLength: 100),
                        Title = c.String(maxLength: 250),
                        SubTitle = c.String(maxLength: 500),
                        SmallDetailsOne = c.String(maxLength: 500),
                        SmallDetailsTwo = c.String(maxLength: 500),
                        BigDetailsOne = c.String(),
                        ArabicBigDetailsOne = c.String(),
                        BigDetailsTwo = c.String(),
                        PictureOne = c.String(maxLength: 150),
                        PictureTwo = c.String(maxLength: 150),
                        PictureThree = c.String(maxLength: 150),
                        ButtonText = c.String(maxLength: 100),
                        ButtonLink = c.String(maxLength: 200),
                        DeletedDate = c.DateTime(),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.WebHtmlId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.RoleModules", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.RoleModules", "ModuleId", "dbo.Modules");
            DropIndex("dbo.Users", new[] { "RoleId" });
            DropIndex("dbo.RoleModules", new[] { "ModuleId" });
            DropIndex("dbo.RoleModules", new[] { "RoleId" });
            DropTable("dbo.WebHtmls");
            DropTable("dbo.Users");
            DropTable("dbo.UserActivities");
            DropTable("dbo.Roles");
            DropTable("dbo.RoleModules");
            DropTable("dbo.Modules");
            DropTable("dbo.LayoutSettings");
            DropTable("dbo.FeatureCategories");
        }
    }
}
