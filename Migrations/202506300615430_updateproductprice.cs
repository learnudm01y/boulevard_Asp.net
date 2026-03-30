namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateproductprice : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductPrices",
                c => new
                    {
                        ProductPriceId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        ProductQuantity = c.Int(nullable: false),
                        Price = c.Double(nullable: false),
                        Status = c.String(),
                        LastUpdateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ProductPriceId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ProductPrices");
        }
    }
}
