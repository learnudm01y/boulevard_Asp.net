namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addadditionalcolomninservicetypetable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ServiceTypes", "PersoneQuantity", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ServiceTypes", "PersoneQuantity", c => c.String(maxLength: 100));
        }
    }
}
