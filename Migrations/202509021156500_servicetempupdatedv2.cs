namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class servicetempupdatedv2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TempServices", "CategoryImage", c => c.String());
            AddColumn("dbo.TempServices", "Images", c => c.String());
            AddColumn("dbo.TempServices", "City", c => c.String());
            AddColumn("dbo.TempServices", "Country", c => c.String());
            AddColumn("dbo.TempServices", "AboutUsAr", c => c.String());
            AddColumn("dbo.TempServices", "ScopeOfServiceAr", c => c.String());
            AddColumn("dbo.TempServices", "DescriptionAr", c => c.String());
            AddColumn("dbo.TempServices", "NameAr", c => c.String());
            AddColumn("dbo.TempServices", "CategoryArabic", c => c.String());
            AddColumn("dbo.TempServices", "SubCategoryArabic", c => c.String());
            AddColumn("dbo.TempServices", "Longitute", c => c.String(maxLength: 100));
            AddColumn("dbo.TempServices", "TypeImage", c => c.String(maxLength: 100));
            AddColumn("dbo.TempServices", "TypeLatitute", c => c.String(maxLength: 100));
            AddColumn("dbo.TempServices", "TypeLogitute", c => c.String(maxLength: 100));
            AddColumn("dbo.TempServices", "AirportName", c => c.String());
            AddColumn("dbo.TempServices", "AirportNameArabic", c => c.String());
            AddColumn("dbo.TempServices", "AirportCode", c => c.String());
            AddColumn("dbo.TempServices", "AmenitiesName", c => c.String());
            AddColumn("dbo.TempServices", "AmenitiesNameArabic", c => c.String());
            AddColumn("dbo.TempServices", "AmenitiesImage", c => c.String());
            AddColumn("dbo.TempServices", "landmarkName", c => c.String());
            AddColumn("dbo.TempServices", "landmarkNameArabic", c => c.String());
            AddColumn("dbo.TempServices", "landmarkNameDistance", c => c.String());
            AddColumn("dbo.TempServices", "landmarkLatitute", c => c.String());
            AddColumn("dbo.TempServices", "landmarkLongitute", c => c.String());
            AlterColumn("dbo.TempServices", "PersoneQuantity", c => c.String());
            AlterColumn("dbo.TempServices", "AdultQuantity", c => c.String());
            AlterColumn("dbo.TempServices", "ChildrenQuantity", c => c.String());
            AlterColumn("dbo.TempServices", "TypeServiceHour", c => c.String());
            AlterColumn("dbo.TempServices", "TypeServiceMin", c => c.String());
            AlterColumn("dbo.TempServices", "TypePrice", c => c.String());
            DropColumn("dbo.TempServices", "ProductImages");
            DropColumn("dbo.TempServices", "Image");
            DropColumn("dbo.TempServices", "Logitute");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TempServices", "Logitute", c => c.String(maxLength: 100));
            AddColumn("dbo.TempServices", "Image", c => c.String(maxLength: 100));
            AddColumn("dbo.TempServices", "ProductImages", c => c.String());
            AlterColumn("dbo.TempServices", "TypePrice", c => c.Double(nullable: false));
            AlterColumn("dbo.TempServices", "TypeServiceMin", c => c.Int(nullable: false));
            AlterColumn("dbo.TempServices", "TypeServiceHour", c => c.Int(nullable: false));
            AlterColumn("dbo.TempServices", "ChildrenQuantity", c => c.Int(nullable: false));
            AlterColumn("dbo.TempServices", "AdultQuantity", c => c.Int(nullable: false));
            AlterColumn("dbo.TempServices", "PersoneQuantity", c => c.Int(nullable: false));
            DropColumn("dbo.TempServices", "landmarkLongitute");
            DropColumn("dbo.TempServices", "landmarkLatitute");
            DropColumn("dbo.TempServices", "landmarkNameDistance");
            DropColumn("dbo.TempServices", "landmarkNameArabic");
            DropColumn("dbo.TempServices", "landmarkName");
            DropColumn("dbo.TempServices", "AmenitiesImage");
            DropColumn("dbo.TempServices", "AmenitiesNameArabic");
            DropColumn("dbo.TempServices", "AmenitiesName");
            DropColumn("dbo.TempServices", "AirportCode");
            DropColumn("dbo.TempServices", "AirportNameArabic");
            DropColumn("dbo.TempServices", "AirportName");
            DropColumn("dbo.TempServices", "TypeLogitute");
            DropColumn("dbo.TempServices", "TypeLatitute");
            DropColumn("dbo.TempServices", "TypeImage");
            DropColumn("dbo.TempServices", "Longitute");
            DropColumn("dbo.TempServices", "SubCategoryArabic");
            DropColumn("dbo.TempServices", "CategoryArabic");
            DropColumn("dbo.TempServices", "NameAr");
            DropColumn("dbo.TempServices", "DescriptionAr");
            DropColumn("dbo.TempServices", "ScopeOfServiceAr");
            DropColumn("dbo.TempServices", "AboutUsAr");
            DropColumn("dbo.TempServices", "Country");
            DropColumn("dbo.TempServices", "City");
            DropColumn("dbo.TempServices", "Images");
            DropColumn("dbo.TempServices", "CategoryImage");
        }
    }
}
