namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatefavouriteProductandservice : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FavouriteProducts",
                c => new
                    {
                        FavouriteProductId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        MemberId = c.Long(nullable: false),
                        FeatureCategoryId = c.Int(nullable: false),
                        Status = c.Boolean(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.FavouriteProductId)
                .ForeignKey("dbo.FeatureCategories", t => t.FeatureCategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Members", t => t.MemberId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.MemberId)
                .Index(t => t.FeatureCategoryId);
            
            CreateTable(
                "dbo.FavouriteServices",
                c => new
                    {
                        FavouriteServiceId = c.Int(nullable: false, identity: true),
                        ServiceId = c.Int(),
                        ServiceTypeId = c.Int(nullable: false),
                        MemberId = c.Long(nullable: false),
                        FeatureCategoryId = c.Int(nullable: false),
                        Status = c.Boolean(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.FavouriteServiceId)
                .ForeignKey("dbo.FeatureCategories", t => t.FeatureCategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Members", t => t.MemberId, cascadeDelete: true)
                .Index(t => t.MemberId)
                .Index(t => t.FeatureCategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FavouriteServices", "MemberId", "dbo.Members");
            DropForeignKey("dbo.FavouriteServices", "FeatureCategoryId", "dbo.FeatureCategories");
            DropForeignKey("dbo.FavouriteProducts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.FavouriteProducts", "MemberId", "dbo.Members");
            DropForeignKey("dbo.FavouriteProducts", "FeatureCategoryId", "dbo.FeatureCategories");
            DropIndex("dbo.FavouriteServices", new[] { "FeatureCategoryId" });
            DropIndex("dbo.FavouriteServices", new[] { "MemberId" });
            DropIndex("dbo.FavouriteProducts", new[] { "FeatureCategoryId" });
            DropIndex("dbo.FavouriteProducts", new[] { "MemberId" });
            DropIndex("dbo.FavouriteProducts", new[] { "ProductId" });
            DropTable("dbo.FavouriteServices");
            DropTable("dbo.FavouriteProducts");
        }
    }
}
