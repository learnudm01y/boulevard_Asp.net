namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateorderrequestdetailsproductpriceid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderRequestProductDetails", "ProductPriceId", c => c.Int());
            CreateIndex("dbo.OrderRequestProductDetails", "ProductPriceId");
            AddForeignKey("dbo.OrderRequestProductDetails", "ProductPriceId", "dbo.ProductPrices", "ProductPriceId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderRequestProductDetails", "ProductPriceId", "dbo.ProductPrices");
            DropIndex("dbo.OrderRequestProductDetails", new[] { "ProductPriceId" });
            DropColumn("dbo.OrderRequestProductDetails", "ProductPriceId");
        }
    }
}
