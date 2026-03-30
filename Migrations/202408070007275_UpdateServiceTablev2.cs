namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateServiceTablev2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "Ratings", c => c.Double(nullable: false));
            AddColumn("dbo.Services", "CheckInTime", c => c.Time(nullable: false, precision: 7));
            AddColumn("dbo.Services", "CheckOutTime", c => c.Time(nullable: false, precision: 7));
            AddColumn("dbo.Services", "PropertyType", c => c.String(maxLength: 100));
            AddColumn("dbo.Services", "Latitute", c => c.String(maxLength: 100));
            AddColumn("dbo.Services", "Logitute", c => c.String(maxLength: 100));
            AddColumn("dbo.Services", "AboutUs", c => c.String());
            AddColumn("dbo.Services", "SpokenLanguages", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "SpokenLanguages");
            DropColumn("dbo.Services", "AboutUs");
            DropColumn("dbo.Services", "Logitute");
            DropColumn("dbo.Services", "Latitute");
            DropColumn("dbo.Services", "PropertyType");
            DropColumn("dbo.Services", "CheckOutTime");
            DropColumn("dbo.Services", "CheckInTime");
            DropColumn("dbo.Services", "Ratings");
        }
    }
}
