namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateservicetemptableboolispackage : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TempServices", "IsPackage", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TempServices", "IsPackage", c => c.String());
        }
    }
}
