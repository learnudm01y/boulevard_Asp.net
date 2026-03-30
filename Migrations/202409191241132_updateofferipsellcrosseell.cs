namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateofferipsellcrosseell : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BrandOffers",
                c => new
                    {
                        BrandOfferId = c.Int(nullable: false, identity: true),
                        BrandId = c.Int(nullable: false),
                        OfferInformationId = c.Int(),
                    })
                .PrimaryKey(t => t.BrandOfferId)
                .ForeignKey("dbo.Brands", t => t.BrandId, cascadeDelete: true)
                .ForeignKey("dbo.OfferInformations", t => t.OfferInformationId)
                .Index(t => t.BrandId)
                .Index(t => t.OfferInformationId);
            
            CreateTable(
                "dbo.OfferInformations",
                c => new
                    {
                        OfferInformationId = c.Int(nullable: false, identity: true),
                        OfferInformationKey = c.Guid(nullable: false),
                        Title = c.String(maxLength: 250),
                        Description = c.String(),
                        FeatureCategoryId = c.Int(nullable: false),
                        IsBrand = c.Boolean(nullable: false),
                        IsCategory = c.Boolean(nullable: false),
                        IsProduct = c.Boolean(nullable: false),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.OfferInformationId)
                .ForeignKey("dbo.FeatureCategories", t => t.FeatureCategoryId, cascadeDelete: true)
                .Index(t => t.FeatureCategoryId);
            
            CreateTable(
                "dbo.CategoryOffers",
                c => new
                    {
                        CategoryOfferId = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        OfferInformationId = c.Int(),
                    })
                .PrimaryKey(t => t.CategoryOfferId)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.OfferInformations", t => t.OfferInformationId)
                .Index(t => t.CategoryId)
                .Index(t => t.OfferInformationId);
            
            CreateTable(
                "dbo.CrosssellFeatures",
                c => new
                    {
                        CrosssellFeaturesId = c.Int(nullable: false, identity: true),
                        CrosssellFeaturesType = c.String(maxLength: 50),
                        CrosssellFeaturesTypeId = c.Int(nullable: false),
                        RelatedFeatureId = c.Int(nullable: false),
                        FeatureCategoryId = c.Int(nullable: false),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.CrosssellFeaturesId)
                .ForeignKey("dbo.FeatureCategories", t => t.FeatureCategoryId, cascadeDelete: true)
                .Index(t => t.FeatureCategoryId);
            
            CreateTable(
                "dbo.OfferBanners",
                c => new
                    {
                        OfferBannerId = c.Int(nullable: false, identity: true),
                        BannerImage = c.String(),
                        OfferInformationId = c.Int(),
                        IsFeature = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.OfferBannerId)
                .ForeignKey("dbo.OfferInformations", t => t.OfferInformationId)
                .Index(t => t.OfferInformationId);
            
            CreateTable(
                "dbo.ProductOffers",
                c => new
                    {
                        ProductOfferId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        IsFeature = c.Boolean(nullable: false),
                        OfferInformationId = c.Int(),
                    })
                .PrimaryKey(t => t.ProductOfferId)
                .ForeignKey("dbo.OfferInformations", t => t.OfferInformationId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.OfferInformationId);
            
            CreateTable(
                "dbo.ServiceOffers",
                c => new
                    {
                        ServiceOffersId = c.Int(nullable: false, identity: true),
                        ServiceId = c.Int(nullable: false),
                        IsFeature = c.Boolean(nullable: false),
                        OfferInformationId = c.Int(),
                    })
                .PrimaryKey(t => t.ServiceOffersId)
                .ForeignKey("dbo.OfferInformations", t => t.OfferInformationId)
                .ForeignKey("dbo.Services", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.ServiceId)
                .Index(t => t.OfferInformationId);
            
            CreateTable(
                "dbo.UpsellFeatures",
                c => new
                    {
                        UpsellFeaturesId = c.Int(nullable: false, identity: true),
                        UpsellFeaturesType = c.String(maxLength: 50),
                        UpsellFeaturesTypeId = c.Int(nullable: false),
                        RelatedFeatureId = c.Int(nullable: false),
                        FeatureCategoryId = c.Int(nullable: false),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.UpsellFeaturesId)
                .ForeignKey("dbo.FeatureCategories", t => t.FeatureCategoryId, cascadeDelete: true)
                .Index(t => t.FeatureCategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UpsellFeatures", "FeatureCategoryId", "dbo.FeatureCategories");
            DropForeignKey("dbo.ServiceOffers", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.ServiceOffers", "OfferInformationId", "dbo.OfferInformations");
            DropForeignKey("dbo.ProductOffers", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductOffers", "OfferInformationId", "dbo.OfferInformations");
            DropForeignKey("dbo.OfferBanners", "OfferInformationId", "dbo.OfferInformations");
            DropForeignKey("dbo.CrosssellFeatures", "FeatureCategoryId", "dbo.FeatureCategories");
            DropForeignKey("dbo.CategoryOffers", "OfferInformationId", "dbo.OfferInformations");
            DropForeignKey("dbo.CategoryOffers", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.BrandOffers", "OfferInformationId", "dbo.OfferInformations");
            DropForeignKey("dbo.OfferInformations", "FeatureCategoryId", "dbo.FeatureCategories");
            DropForeignKey("dbo.BrandOffers", "BrandId", "dbo.Brands");
            DropIndex("dbo.UpsellFeatures", new[] { "FeatureCategoryId" });
            DropIndex("dbo.ServiceOffers", new[] { "OfferInformationId" });
            DropIndex("dbo.ServiceOffers", new[] { "ServiceId" });
            DropIndex("dbo.ProductOffers", new[] { "OfferInformationId" });
            DropIndex("dbo.ProductOffers", new[] { "ProductId" });
            DropIndex("dbo.OfferBanners", new[] { "OfferInformationId" });
            DropIndex("dbo.CrosssellFeatures", new[] { "FeatureCategoryId" });
            DropIndex("dbo.CategoryOffers", new[] { "OfferInformationId" });
            DropIndex("dbo.CategoryOffers", new[] { "CategoryId" });
            DropIndex("dbo.OfferInformations", new[] { "FeatureCategoryId" });
            DropIndex("dbo.BrandOffers", new[] { "OfferInformationId" });
            DropIndex("dbo.BrandOffers", new[] { "BrandId" });
            DropTable("dbo.UpsellFeatures");
            DropTable("dbo.ServiceOffers");
            DropTable("dbo.ProductOffers");
            DropTable("dbo.OfferBanners");
            DropTable("dbo.CrosssellFeatures");
            DropTable("dbo.CategoryOffers");
            DropTable("dbo.OfferInformations");
            DropTable("dbo.BrandOffers");
        }
    }
}
