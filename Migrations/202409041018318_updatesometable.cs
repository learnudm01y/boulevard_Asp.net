namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatesometable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FaqServices",
                c => new
                    {
                        FaqServiceId = c.Int(nullable: false, identity: true),
                        FAQKey = c.Guid(nullable: false),
                        FaqTitle = c.String(),
                        FaqDescription = c.String(),
                        FeatureType = c.String(),
                        FeatureTypeId = c.Int(nullable: false),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.FaqServiceId);
            
            CreateTable(
                "dbo.MemberVehicalInfoes",
                c => new
                    {
                        MemberVehicalInfoId = c.Int(nullable: false, identity: true),
                        BrandId = c.Int(nullable: false),
                        VehicalModelId = c.Int(nullable: false),
                        Year = c.String(maxLength: 50),
                        PlateNo = c.String(maxLength: 500),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.MemberVehicalInfoId)
                .ForeignKey("dbo.Brands", t => t.BrandId, cascadeDelete: true)
                .ForeignKey("dbo.VehicalModels", t => t.VehicalModelId, cascadeDelete: true)
                .Index(t => t.BrandId)
                .Index(t => t.VehicalModelId);
            
            CreateTable(
                "dbo.VehicalModels",
                c => new
                    {
                        VehicalModelId = c.Int(nullable: false, identity: true),
                        VehicalModelKey = c.Guid(nullable: false),
                        VehicalModelName = c.String(maxLength: 250),
                        ModelDetails = c.String(),
                        BrandId = c.Int(),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.VehicalModelId)
                .ForeignKey("dbo.Brands", t => t.BrandId)
                .Index(t => t.BrandId);
            
            AddColumn("dbo.OrderRequestServices", "MemberAddressId", c => c.Long());
            AddColumn("dbo.OrderRequestServices", "MemberVehicalInfoId", c => c.Int());
            AddColumn("dbo.ServiceTypes", "AdultQuantity", c => c.Int(nullable: false));
            AddColumn("dbo.ServiceTypes", "ChildrenQuantity", c => c.Int(nullable: false));
            AddColumn("dbo.ServiceTypes", "ServiceHour", c => c.Int(nullable: false));
            AddColumn("dbo.WebHtmls", "FeatureType", c => c.String());
            AddColumn("dbo.WebHtmls", "FeatureTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.OrderRequestServices", "MemberAddressId");
            CreateIndex("dbo.OrderRequestServices", "MemberVehicalInfoId");
            AddForeignKey("dbo.OrderRequestServices", "MemberAddressId", "dbo.MemberAddresses", "MemberAddressId");
            AddForeignKey("dbo.OrderRequestServices", "MemberVehicalInfoId", "dbo.MemberVehicalInfoes", "MemberVehicalInfoId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderRequestServices", "MemberVehicalInfoId", "dbo.MemberVehicalInfoes");
            DropForeignKey("dbo.OrderRequestServices", "MemberAddressId", "dbo.MemberAddresses");
            DropForeignKey("dbo.MemberVehicalInfoes", "VehicalModelId", "dbo.VehicalModels");
            DropForeignKey("dbo.VehicalModels", "BrandId", "dbo.Brands");
            DropForeignKey("dbo.MemberVehicalInfoes", "BrandId", "dbo.Brands");
            DropIndex("dbo.OrderRequestServices", new[] { "MemberVehicalInfoId" });
            DropIndex("dbo.OrderRequestServices", new[] { "MemberAddressId" });
            DropIndex("dbo.VehicalModels", new[] { "BrandId" });
            DropIndex("dbo.MemberVehicalInfoes", new[] { "VehicalModelId" });
            DropIndex("dbo.MemberVehicalInfoes", new[] { "BrandId" });
            DropColumn("dbo.WebHtmls", "FeatureTypeId");
            DropColumn("dbo.WebHtmls", "FeatureType");
            DropColumn("dbo.ServiceTypes", "ServiceHour");
            DropColumn("dbo.ServiceTypes", "ChildrenQuantity");
            DropColumn("dbo.ServiceTypes", "AdultQuantity");
            DropColumn("dbo.OrderRequestServices", "MemberVehicalInfoId");
            DropColumn("dbo.OrderRequestServices", "MemberAddressId");
            DropTable("dbo.VehicalModels");
            DropTable("dbo.MemberVehicalInfoes");
            DropTable("dbo.FaqServices");
        }
    }
}
