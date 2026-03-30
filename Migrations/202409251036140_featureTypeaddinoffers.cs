namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class featureTypeaddinoffers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OfferInformations", "FeatureType", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OfferInformations", "FeatureType");
        }
    }
}
