namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingproducttable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Airports", "IsSelected", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Airports", "IsSelected");
        }
    }
}
