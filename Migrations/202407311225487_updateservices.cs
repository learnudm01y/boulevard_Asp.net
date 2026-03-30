namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateservices : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ServiceAmenities",
                c => new
                    {
                        ServiceAmenityId = c.Int(nullable: false, identity: true),
                        ServiceId = c.Int(nullable: false),
                        AmenitiesName = c.String(maxLength: 100),
                        AmenitiesLogo = c.String(maxLength: 100),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ServiceAmenityId)
                .ForeignKey("dbo.Services", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.ServiceId);
            
            CreateTable(
                "dbo.Services",
                c => new
                    {
                        ServiceId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Description = c.String(),
                        ServiceHour = c.Int(nullable: false),
                        FeatureCategoryId = c.Int(nullable: false),
                        CityId = c.Int(),
                        CountryId = c.Int(),
                        Address = c.String(),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ServiceId)
                .ForeignKey("dbo.Cities", t => t.CityId)
                .ForeignKey("dbo.Countries", t => t.CountryId)
                .ForeignKey("dbo.FeatureCategories", t => t.FeatureCategoryId, cascadeDelete: true)
                .Index(t => t.FeatureCategoryId)
                .Index(t => t.CityId)
                .Index(t => t.CountryId);
            
            CreateTable(
                "dbo.ServiceImages",
                c => new
                    {
                        ServiceImageId = c.Int(nullable: false, identity: true),
                        ServiceId = c.Int(nullable: false),
                        Image = c.String(maxLength: 180),
                        IsFeature = c.Boolean(nullable: false),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ServiceImageId)
                .ForeignKey("dbo.Services", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.ServiceId);
            
            CreateTable(
                "dbo.ServiceTypes",
                c => new
                    {
                        ServiceTypeId = c.Int(nullable: false, identity: true),
                        ServiceTypeName = c.String(maxLength: 250),
                        PersoneQuantity = c.String(maxLength: 100),
                        Description = c.String(),
                        Size = c.String(maxLength: 100),
                        Image = c.String(maxLength: 100),
                        Price = c.Double(nullable: false),
                        ServiceId = c.Int(nullable: false),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ServiceTypeId)
                .ForeignKey("dbo.Services", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.ServiceId);
            
            CreateTable(
                "dbo.UserReviews",
                c => new
                    {
                        UserReviewId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        UserType = c.String(),
                        FeatureType = c.String(),
                        FeatureTypeId = c.Int(nullable: false),
                        Rating = c.Int(nullable: false),
                        Comment = c.String(),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserReviewId);
            
            CreateTable(
                "dbo.UserReviewImages",
                c => new
                    {
                        UserReviewImageId = c.Int(nullable: false, identity: true),
                        UserReviewId = c.Int(nullable: false),
                        Image = c.String(maxLength: 180),
                    })
                .PrimaryKey(t => t.UserReviewImageId)
                .ForeignKey("dbo.UserReviews", t => t.UserReviewId, cascadeDelete: true)
                .Index(t => t.UserReviewId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserReviewImages", "UserReviewId", "dbo.UserReviews");
            DropForeignKey("dbo.ServiceTypes", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.ServiceImages", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.ServiceAmenities", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.Services", "FeatureCategoryId", "dbo.FeatureCategories");
            DropForeignKey("dbo.Services", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.Services", "CityId", "dbo.Cities");
            DropIndex("dbo.UserReviewImages", new[] { "UserReviewId" });
            DropIndex("dbo.ServiceTypes", new[] { "ServiceId" });
            DropIndex("dbo.ServiceImages", new[] { "ServiceId" });
            DropIndex("dbo.Services", new[] { "CountryId" });
            DropIndex("dbo.Services", new[] { "CityId" });
            DropIndex("dbo.Services", new[] { "FeatureCategoryId" });
            DropIndex("dbo.ServiceAmenities", new[] { "ServiceId" });
            DropTable("dbo.UserReviewImages");
            DropTable("dbo.UserReviews");
            DropTable("dbo.ServiceTypes");
            DropTable("dbo.ServiceImages");
            DropTable("dbo.Services");
            DropTable("dbo.ServiceAmenities");
        }
    }
}
