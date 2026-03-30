namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateorderstatusforcourierv2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderStatus", "NameAr", c => c.String(maxLength: 100));
            AddColumn("dbo.OrderStatus", "PublicNameAr", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderStatus", "PublicNameAr");
            DropColumn("dbo.OrderStatus", "NameAr");
        }
    }
}
