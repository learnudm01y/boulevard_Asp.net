namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateproductandservicerequest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderRequestProducts", "FeatureCategoryId", c => c.Int());
            AddColumn("dbo.OrderRequestServices", "FeatureCategoryId", c => c.Int());
            CreateIndex("dbo.OrderRequestProducts", "FeatureCategoryId");
            CreateIndex("dbo.OrderRequestServices", "FeatureCategoryId");
            AddForeignKey("dbo.OrderRequestProducts", "FeatureCategoryId", "dbo.FeatureCategories", "FeatureCategoryId");
            AddForeignKey("dbo.OrderRequestServices", "FeatureCategoryId", "dbo.FeatureCategories", "FeatureCategoryId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderRequestServices", "FeatureCategoryId", "dbo.FeatureCategories");
            DropForeignKey("dbo.OrderRequestProducts", "FeatureCategoryId", "dbo.FeatureCategories");
            DropIndex("dbo.OrderRequestServices", new[] { "FeatureCategoryId" });
            DropIndex("dbo.OrderRequestProducts", new[] { "FeatureCategoryId" });
            DropColumn("dbo.OrderRequestServices", "FeatureCategoryId");
            DropColumn("dbo.OrderRequestProducts", "FeatureCategoryId");
        }
    }
}
