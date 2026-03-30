namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updateservicetypetablewithispackage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceTypes", "IsPackage", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceTypes", "IsPackage");
        }
    }
}
