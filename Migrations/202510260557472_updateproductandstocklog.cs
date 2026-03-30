namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateproductandstocklog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductPrices", "ProductStock", c => c.Int(nullable: false));
            AddColumn("dbo.StockLogs", "ProductPriceId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockLogs", "ProductPriceId");
            DropColumn("dbo.ProductPrices", "ProductStock");
        }
    }
}
