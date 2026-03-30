namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updateservicetypetablewithcityandcountry : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceTypes", "CityId", c => c.Int());
            AddColumn("dbo.ServiceTypes", "CountryId", c => c.Int());
            AddColumn("dbo.ServiceTypes", "Address", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceTypes", "Address");
            DropColumn("dbo.ServiceTypes", "CountryId");
            DropColumn("dbo.ServiceTypes", "CityId");
        }
    }
}
