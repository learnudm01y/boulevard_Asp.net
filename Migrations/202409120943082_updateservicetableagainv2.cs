namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateservicetableagainv2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OrderRequestServices", "BookingTime", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OrderRequestServices", "BookingTime", c => c.String());
        }
    }
}
