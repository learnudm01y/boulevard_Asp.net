namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productupdateandtempproductsv2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TempProducts", "DeliveryInfo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TempProducts", "DeliveryInfo");
        }
    }
}
