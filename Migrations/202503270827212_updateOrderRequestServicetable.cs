namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateOrderRequestServicetable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderRequestServices", "FromAirportId", c => c.Int(nullable: false));
            AddColumn("dbo.OrderRequestServices", "ToAirportId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderRequestServices", "ToAirportId");
            DropColumn("dbo.OrderRequestServices", "FromAirportId");
        }
    }
}
