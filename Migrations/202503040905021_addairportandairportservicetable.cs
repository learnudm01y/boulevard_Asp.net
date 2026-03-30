namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addairportandairportservicetable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Airports",
                c => new
                    {
                        AirportId = c.Int(nullable: false, identity: true),
                        AirportCode = c.String(),
                        AirportName = c.String(),
                        CountryId = c.Int(nullable: false),
                        CityId = c.Int(nullable: false),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.AirportId)
                .ForeignKey("dbo.Cities", t => t.CityId, cascadeDelete: true)
                .Index(t => t.CityId);
            
            CreateTable(
                "dbo.AirportServices",
                c => new
                    {
                        AirportServiceId = c.Int(nullable: false, identity: true),
                        ServiceId = c.Int(nullable: false),
                        AirportId = c.Int(nullable: false),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.AirportServiceId)
                .ForeignKey("dbo.Airports", t => t.AirportId, cascadeDelete: true)
                .ForeignKey("dbo.Services", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.ServiceId)
                .Index(t => t.AirportId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AirportServices", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.AirportServices", "AirportId", "dbo.Airports");
            DropForeignKey("dbo.Airports", "CityId", "dbo.Cities");
            DropIndex("dbo.AirportServices", new[] { "AirportId" });
            DropIndex("dbo.AirportServices", new[] { "ServiceId" });
            DropIndex("dbo.Airports", new[] { "CityId" });
            DropTable("dbo.AirportServices");
            DropTable("dbo.Airports");
        }
    }
}
