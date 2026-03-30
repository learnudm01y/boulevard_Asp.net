namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetempproductwithimagecategoryt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TempProducts", "CategoryImage", c => c.String());
            AddColumn("dbo.TempProducts", "SubCategoryImage", c => c.String());
            AddColumn("dbo.TempProducts", "SubSubCategoryImage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TempProducts", "SubSubCategoryImage");
            DropColumn("dbo.TempProducts", "SubCategoryImage");
            DropColumn("dbo.TempProducts", "CategoryImage");
        }
    }
}
