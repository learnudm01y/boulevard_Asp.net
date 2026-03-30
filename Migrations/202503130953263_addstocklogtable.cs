namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addstocklogtable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StockLogs",
                c => new
                    {
                        StockLogId = c.Int(nullable: false, identity: true),
                        StockKey = c.Guid(nullable: false),
                        ProductId = c.Int(nullable: false),
                        StockDate = c.DateTime(),
                        StockIn = c.Int(nullable: false),
                        StockOut = c.Int(nullable: false),
                        CreateDate = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        StockType = c.String(),
                        OrderMasterId = c.Int(nullable: false),
                        UserType = c.String(),
                        FeatureCategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StockLogId)
                .ForeignKey("dbo.FeatureCategories", t => t.FeatureCategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.FeatureCategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StockLogs", "ProductId", "dbo.Products");
            DropForeignKey("dbo.StockLogs", "FeatureCategoryId", "dbo.FeatureCategories");
            DropIndex("dbo.StockLogs", new[] { "FeatureCategoryId" });
            DropIndex("dbo.StockLogs", new[] { "ProductId" });
            DropTable("dbo.StockLogs");
        }
    }
}
