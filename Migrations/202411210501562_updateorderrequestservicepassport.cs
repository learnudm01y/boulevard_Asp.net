namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateorderrequestservicepassport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderRequestServices", "PassportCopy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderRequestServices", "PassportCopy");
        }
    }
}
