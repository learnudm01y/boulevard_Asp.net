namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetempproductagainarabic : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TempProducts", "ProductNameArabic", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TempProducts", "ProductNameArabic");
        }
    }
}
