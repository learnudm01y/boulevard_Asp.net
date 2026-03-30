namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productupdateandtempproductsv3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TempProducts", "BrandArabic", c => c.String());
            AddColumn("dbo.TempProducts", "CategoryArabic", c => c.String());
            AddColumn("dbo.TempProducts", "SubCategoryArabic", c => c.String());
            AddColumn("dbo.TempProducts", "SubSubCategoryArabic", c => c.String());
            AddColumn("dbo.TempProducts", "ItemDescArabic", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TempProducts", "ItemDescArabic");
            DropColumn("dbo.TempProducts", "SubSubCategoryArabic");
            DropColumn("dbo.TempProducts", "SubCategoryArabic");
            DropColumn("dbo.TempProducts", "CategoryArabic");
            DropColumn("dbo.TempProducts", "BrandArabic");
        }
    }
}
