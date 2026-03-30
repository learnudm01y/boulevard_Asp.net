namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isserviceaddinoffers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OfferInformations", "IsService", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OfferInformations", "IsService");
        }
    }
}
