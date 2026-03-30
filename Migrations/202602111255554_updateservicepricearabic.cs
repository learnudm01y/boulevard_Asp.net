namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateservicepricearabic : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceTypes", "ServicePriceAr", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceTypes", "ServicePriceAr");
        }
    }
}
