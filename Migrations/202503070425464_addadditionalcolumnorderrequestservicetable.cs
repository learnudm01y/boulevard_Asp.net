namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addadditionalcolumnorderrequestservicetable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderRequestServices", "IsPackage", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderRequestServices", "IsPackage");
        }
    }
}
