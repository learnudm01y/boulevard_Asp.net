namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productupdateandtempproducts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "AttributeCode", c => c.String(maxLength: 250));
            AddColumn("dbo.Products", "AttributeName", c => c.String(maxLength: 250));
            AddColumn("dbo.Products", "AttributeNameArabic", c => c.String(maxLength: 250));
            AddColumn("dbo.Products", "DeliveryInfoArabic", c => c.String());
            AddColumn("dbo.TempProducts", "AttributeNameArabic", c => c.String(maxLength: 250));
            AddColumn("dbo.TempProducts", "DeliveryInfoArabic", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TempProducts", "DeliveryInfoArabic");
            DropColumn("dbo.TempProducts", "AttributeNameArabic");
            DropColumn("dbo.Products", "DeliveryInfoArabic");
            DropColumn("dbo.Products", "AttributeNameArabic");
            DropColumn("dbo.Products", "AttributeName");
            DropColumn("dbo.Products", "AttributeCode");
        }
    }
}
