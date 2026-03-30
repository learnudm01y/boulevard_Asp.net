namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateproductcartpriceid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Carts", "ProductPriceId", c => c.Int(nullable: false));
            AlterColumn("dbo.ProductPrices", "ProductQuantity", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductPrices", "ProductQuantity", c => c.Int(nullable: false));
            DropColumn("dbo.Carts", "ProductPriceId");
        }
    }
}
