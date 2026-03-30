namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateanothertable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceLandmarks", "NameAr", c => c.String());
            AddColumn("dbo.TempServices", "IsPackage", c => c.String());
            AddColumn("dbo.TempServices", "LandmarkAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TempServices", "LandmarkAddress");
            DropColumn("dbo.TempServices", "IsPackage");
            DropColumn("dbo.ServiceLandmarks", "NameAr");
        }
    }
}
