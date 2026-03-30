namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateServiceList : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "ScopeOfService", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "ScopeOfService");
        }
    }
}
