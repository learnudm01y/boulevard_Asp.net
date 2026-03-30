namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateserviceandservicetypetable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "ServiceMin", c => c.Int(nullable: false));
            AddColumn("dbo.ServiceTypes", "ServiceMin", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceTypes", "ServiceMin");
            DropColumn("dbo.Services", "ServiceMin");
        }
    }
}
