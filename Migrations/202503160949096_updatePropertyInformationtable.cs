namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatePropertyInformationtable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PropertyInformations", "CityId", c => c.Int());
            AddColumn("dbo.PropertyInformations", "CountryId", c => c.Int());
            CreateIndex("dbo.PropertyInformations", "CityId");
            CreateIndex("dbo.PropertyInformations", "CountryId");
            AddForeignKey("dbo.PropertyInformations", "CityId", "dbo.Cities", "CityId");
            AddForeignKey("dbo.PropertyInformations", "CountryId", "dbo.Countries", "CountryId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PropertyInformations", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.PropertyInformations", "CityId", "dbo.Cities");
            DropIndex("dbo.PropertyInformations", new[] { "CountryId" });
            DropIndex("dbo.PropertyInformations", new[] { "CityId" });
            DropColumn("dbo.PropertyInformations", "CountryId");
            DropColumn("dbo.PropertyInformations", "CityId");
        }
    }
}
