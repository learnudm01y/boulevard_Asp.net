namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatefeaturecategorytype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FeatureCategories", "FeatureType", c => c.String(maxLength: 100));
            AddColumn("dbo.Products", "BrandId", c => c.Int());
            CreateIndex("dbo.Products", "BrandId");
            AddForeignKey("dbo.Products", "BrandId", "dbo.Brands", "BrandId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "BrandId", "dbo.Brands");
            DropIndex("dbo.Products", new[] { "BrandId" });
            DropColumn("dbo.Products", "BrandId");
            DropColumn("dbo.FeatureCategories", "FeatureType");
        }
    }
}
