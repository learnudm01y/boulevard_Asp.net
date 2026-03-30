namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateservicetableagain : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderRequestServices", "BookingTime", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderRequestServices", "BookingTime");
        }
    }
}
