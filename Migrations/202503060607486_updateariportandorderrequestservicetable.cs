namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateariportandorderrequestservicetable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Airports", "AirportKey", c => c.Guid(nullable: false));
            AddColumn("dbo.Airports", "Details", c => c.String());
            AddColumn("dbo.OrderRequestServices", "InTime", c => c.DateTime());
            AddColumn("dbo.OrderRequestServices", "OutTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderRequestServices", "OutTime");
            DropColumn("dbo.OrderRequestServices", "InTime");
            DropColumn("dbo.Airports", "Details");
            DropColumn("dbo.Airports", "AirportKey");
        }
    }
}
