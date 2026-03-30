namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sometableadd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        CategoryKey = c.Guid(nullable: false),
                        CategoryName = c.String(maxLength: 250),
                        CategoryDescription = c.String(),
                        Image = c.String(maxLength: 180),
                        ParentId = c.Int(),
                        FeatureCategoryId = c.Int(nullable: false),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.CategoryId)
                .ForeignKey("dbo.FeatureCategories", t => t.FeatureCategoryId, cascadeDelete: true)
                .Index(t => t.FeatureCategoryId);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        CityId = c.Int(nullable: false, identity: true),
                        CityKey = c.Guid(nullable: false),
                        CityName = c.String(),
                        CountryId = c.Int(nullable: false),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.CityId)
                .ForeignKey("dbo.Countries", t => t.CountryId, cascadeDelete: true)
                .Index(t => t.CountryId);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        CountryId = c.Int(nullable: false, identity: true),
                        CountryKey = c.Guid(nullable: false),
                        CountryName = c.String(),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.CountryId);
            
            CreateTable(
                "dbo.MemberAddresses",
                c => new
                    {
                        MemberAddressId = c.Long(nullable: false, identity: true),
                        MemberAddressKey = c.Guid(nullable: false),
                        CountryId = c.Int(nullable: false),
                        CityId = c.Int(nullable: false),
                        AddressLine1 = c.String(maxLength: 250),
                        AddressLine2 = c.String(maxLength: 250),
                        NearByAddress = c.String(maxLength: 250),
                        MemberId = c.Long(nullable: false),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.MemberAddressId)
                .ForeignKey("dbo.Members", t => t.MemberId, cascadeDelete: true)
                .Index(t => t.MemberId);
            
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        MemberId = c.Long(nullable: false, identity: true),
                        MemberKey = c.Guid(nullable: false),
                        FirstName = c.String(maxLength: 150),
                        LastName = c.String(maxLength: 150),
                        Email = c.String(maxLength: 250),
                        Phone = c.String(maxLength: 20),
                        Image = c.String(maxLength: 180),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.MemberId);
            
            CreateTable(
                "dbo.ProductCategories",
                c => new
                    {
                        ProductCategoryId = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Status = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.ProductCategoryId)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.CategoryId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        ProductKey = c.Guid(nullable: false),
                        ProductName = c.String(maxLength: 250),
                        ProductSlag = c.String(maxLength: 250),
                        ProductDescription = c.String(),
                        FeatureCategoryId = c.Int(),
                        ProductPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("dbo.FeatureCategories", t => t.FeatureCategoryId)
                .Index(t => t.FeatureCategoryId);
            
            CreateTable(
                "dbo.ProductImages",
                c => new
                    {
                        ProductImageId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        Image = c.String(maxLength: 180),
                        IsFeature = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ProductImageId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            AddColumn("dbo.FeatureCategories", "Image", c => c.String(maxLength: 180));
            AddColumn("dbo.RoleModules", "Status", c => c.String(maxLength: 10));
            AddColumn("dbo.RoleModules", "CreateBy", c => c.Int(nullable: false));
            AddColumn("dbo.RoleModules", "CreateDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.RoleModules", "UpdateBy", c => c.Int());
            AddColumn("dbo.RoleModules", "DeleteBy", c => c.Int());
            AddColumn("dbo.RoleModules", "DeleteDate", c => c.DateTime());
            AddColumn("dbo.Roles", "CreateBy", c => c.Int(nullable: false));
            AddColumn("dbo.Roles", "CreateDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Roles", "UpdateBy", c => c.Int());
            AddColumn("dbo.Roles", "DeleteBy", c => c.Int());
            AddColumn("dbo.Roles", "DeleteDate", c => c.DateTime());
            AddColumn("dbo.WebHtmls", "FeatureCategoryId", c => c.Int());
            AlterColumn("dbo.RoleModules", "UpdateDate", c => c.DateTime());
            AlterColumn("dbo.Roles", "UpdateDate", c => c.DateTime());
            CreateIndex("dbo.WebHtmls", "FeatureCategoryId");
            AddForeignKey("dbo.WebHtmls", "FeatureCategoryId", "dbo.FeatureCategories", "FeatureCategoryId");
            DropColumn("dbo.RoleModules", "UpdatedBy");
            DropColumn("dbo.Roles", "UpdatedBy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Roles", "UpdatedBy", c => c.Int(nullable: false));
            AddColumn("dbo.RoleModules", "UpdatedBy", c => c.Int(nullable: false));
            DropForeignKey("dbo.WebHtmls", "FeatureCategoryId", "dbo.FeatureCategories");
            DropForeignKey("dbo.ProductImages", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductCategories", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "FeatureCategoryId", "dbo.FeatureCategories");
            DropForeignKey("dbo.ProductCategories", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.MemberAddresses", "MemberId", "dbo.Members");
            DropForeignKey("dbo.Cities", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.Categories", "FeatureCategoryId", "dbo.FeatureCategories");
            DropIndex("dbo.WebHtmls", new[] { "FeatureCategoryId" });
            DropIndex("dbo.ProductImages", new[] { "ProductId" });
            DropIndex("dbo.Products", new[] { "FeatureCategoryId" });
            DropIndex("dbo.ProductCategories", new[] { "ProductId" });
            DropIndex("dbo.ProductCategories", new[] { "CategoryId" });
            DropIndex("dbo.MemberAddresses", new[] { "MemberId" });
            DropIndex("dbo.Cities", new[] { "CountryId" });
            DropIndex("dbo.Categories", new[] { "FeatureCategoryId" });
            AlterColumn("dbo.Roles", "UpdateDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.RoleModules", "UpdateDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.WebHtmls", "FeatureCategoryId");
            DropColumn("dbo.Roles", "DeleteDate");
            DropColumn("dbo.Roles", "DeleteBy");
            DropColumn("dbo.Roles", "UpdateBy");
            DropColumn("dbo.Roles", "CreateDate");
            DropColumn("dbo.Roles", "CreateBy");
            DropColumn("dbo.RoleModules", "DeleteDate");
            DropColumn("dbo.RoleModules", "DeleteBy");
            DropColumn("dbo.RoleModules", "UpdateBy");
            DropColumn("dbo.RoleModules", "CreateDate");
            DropColumn("dbo.RoleModules", "CreateBy");
            DropColumn("dbo.RoleModules", "Status");
            DropColumn("dbo.FeatureCategories", "Image");
            DropTable("dbo.ProductImages");
            DropTable("dbo.Products");
            DropTable("dbo.ProductCategories");
            DropTable("dbo.Members");
            DropTable("dbo.MemberAddresses");
            DropTable("dbo.Countries");
            DropTable("dbo.Cities");
            DropTable("dbo.Categories");
        }
    }
}
