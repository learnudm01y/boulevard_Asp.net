namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetempserviceswithbigdetailsv2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TempServices", "SubCategoryImage", c => c.String());
            AddColumn("dbo.TempServices", "SubCategoryIcon", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TempServices", "SubCategoryIcon");
            DropColumn("dbo.TempServices", "SubCategoryImage");
        }
    }
}
