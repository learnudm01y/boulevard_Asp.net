namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class faqservicemodelupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FaqServices", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.FaqServices", "LastUpdate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FaqServices", "LastUpdate");
            DropColumn("dbo.FaqServices", "IsActive");
        }
    }
}
