namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecategoryispackage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "IsPackagecategory", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Categories", "IsPackagecategory");
        }
    }
}
