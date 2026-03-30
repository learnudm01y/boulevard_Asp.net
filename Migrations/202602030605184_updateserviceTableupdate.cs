namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateserviceTableupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "ParentId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "ParentId");
        }
    }
}
