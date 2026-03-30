namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetempproducttable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TempProducts", "SubSubCategory", c => c.String());
            AddColumn("dbo.TempProducts", "ProductType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TempProducts", "ProductType");
            DropColumn("dbo.TempProducts", "SubSubCategory");
        }
    }
}
