namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetempserviceswithbigdetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TempServices", "CategoryIcon", c => c.String());
            AddColumn("dbo.TempServices", "ServiceTypeBigDescription", c => c.String());
            AddColumn("dbo.TempServices", "ServiceTypeBigDescriptionArabic", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TempServices", "ServiceTypeBigDescriptionArabic");
            DropColumn("dbo.TempServices", "ServiceTypeBigDescription");
            DropColumn("dbo.TempServices", "CategoryIcon");
        }
    }
}
