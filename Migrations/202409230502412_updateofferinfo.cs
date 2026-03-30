namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateofferinfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OfferInformations", "StartDate", c => c.DateTime());
            AddColumn("dbo.OfferInformations", "EndDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OfferInformations", "EndDate");
            DropColumn("dbo.OfferInformations", "StartDate");
        }
    }
}
