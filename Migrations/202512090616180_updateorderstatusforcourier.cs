namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateorderstatusforcourier : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderStatus", "CourierStatusName", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderStatus", "CourierStatusName");
        }
    }
}
