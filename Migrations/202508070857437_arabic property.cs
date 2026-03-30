namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class arabicproperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Airports", "AirportNameAr", c => c.String());
            AddColumn("dbo.FeatureCategories", "NameAr", c => c.String());
            AddColumn("dbo.VehicalModels", "VehicalModelNameAr", c => c.String());
            AddColumn("dbo.VehicalModels", "ModelDetailsAr", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VehicalModels", "ModelDetailsAr");
            DropColumn("dbo.VehicalModels", "VehicalModelNameAr");
            DropColumn("dbo.FeatureCategories", "NameAr");
            DropColumn("dbo.Airports", "AirportNameAr");
        }
    }
}
