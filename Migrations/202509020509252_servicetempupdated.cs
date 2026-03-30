namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class servicetempupdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TempServices", "CheckInTime", c => c.String());
            AddColumn("dbo.TempServices", "CheckOutTime", c => c.String());
            AddColumn("dbo.TempServices", "FaqTitle", c => c.String());
            AddColumn("dbo.TempServices", "FaqDescription", c => c.String());
            AddColumn("dbo.TempServices", "FaqTitleAr", c => c.String());
            AddColumn("dbo.TempServices", "FaqDescriptionAr", c => c.String());
            AddColumn("dbo.TempServices", "ServiceTypeName", c => c.String(maxLength: 250));
            AddColumn("dbo.TempServices", "ServiceTypeNameAr", c => c.String(maxLength: 250));
            AddColumn("dbo.TempServices", "PersoneQuantity", c => c.Int(nullable: false));
            AddColumn("dbo.TempServices", "AdultQuantity", c => c.Int(nullable: false));
            AddColumn("dbo.TempServices", "ChildrenQuantity", c => c.Int(nullable: false));
            AddColumn("dbo.TempServices", "TypeDescription", c => c.String());
            AddColumn("dbo.TempServices", "TypeDescriptionAr", c => c.String());
            AddColumn("dbo.TempServices", "TypeServiceHour", c => c.Int(nullable: false));
            AddColumn("dbo.TempServices", "TypeServiceMin", c => c.Int(nullable: false));
            AddColumn("dbo.TempServices", "Size", c => c.String(maxLength: 100));
            AddColumn("dbo.TempServices", "SizeAr", c => c.String(maxLength: 100));
            AddColumn("dbo.TempServices", "Image", c => c.String(maxLength: 100));
            AddColumn("dbo.TempServices", "PaymentType", c => c.String(maxLength: 100));
            AddColumn("dbo.TempServices", "TypePrice", c => c.Double(nullable: false));
            AddColumn("dbo.TempServices", "Latitute", c => c.String(maxLength: 100));
            AddColumn("dbo.TempServices", "Logitute", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TempServices", "Logitute");
            DropColumn("dbo.TempServices", "Latitute");
            DropColumn("dbo.TempServices", "TypePrice");
            DropColumn("dbo.TempServices", "PaymentType");
            DropColumn("dbo.TempServices", "Image");
            DropColumn("dbo.TempServices", "SizeAr");
            DropColumn("dbo.TempServices", "Size");
            DropColumn("dbo.TempServices", "TypeServiceMin");
            DropColumn("dbo.TempServices", "TypeServiceHour");
            DropColumn("dbo.TempServices", "TypeDescriptionAr");
            DropColumn("dbo.TempServices", "TypeDescription");
            DropColumn("dbo.TempServices", "ChildrenQuantity");
            DropColumn("dbo.TempServices", "AdultQuantity");
            DropColumn("dbo.TempServices", "PersoneQuantity");
            DropColumn("dbo.TempServices", "ServiceTypeNameAr");
            DropColumn("dbo.TempServices", "ServiceTypeName");
            DropColumn("dbo.TempServices", "FaqDescriptionAr");
            DropColumn("dbo.TempServices", "FaqTitleAr");
            DropColumn("dbo.TempServices", "FaqDescription");
            DropColumn("dbo.TempServices", "FaqTitle");
            DropColumn("dbo.TempServices", "CheckOutTime");
            DropColumn("dbo.TempServices", "CheckInTime");
        }
    }
}
