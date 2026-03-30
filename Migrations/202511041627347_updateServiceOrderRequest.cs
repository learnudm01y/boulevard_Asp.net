namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateServiceOrderRequest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderRequestProducts", "ProductType", c => c.Int(nullable: false));
            AddColumn("dbo.OrderRequestServices", "DeliveryCharge", c => c.Double(nullable: false));
            AddColumn("dbo.OrderRequestServices", "ServiceCharge", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderRequestServices", "ServiceCharge");
            DropColumn("dbo.OrderRequestServices", "DeliveryCharge");
            DropColumn("dbo.OrderRequestProducts", "ProductType");
        }
    }
}
