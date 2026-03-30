namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateservicetypeserviceprice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceTypes", "ServicePrice", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceTypes", "ServicePrice");
        }
    }
}
