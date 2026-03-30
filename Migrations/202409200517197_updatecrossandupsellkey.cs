namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecrossandupsellkey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CrosssellFeatures", "CrosssellFeaturesKey", c => c.Guid(nullable: false));
            AddColumn("dbo.UpsellFeatures", "UpsellFeaturesKey", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UpsellFeatures", "UpsellFeaturesKey");
            DropColumn("dbo.CrosssellFeatures", "CrosssellFeaturesKey");
        }
    }
}
