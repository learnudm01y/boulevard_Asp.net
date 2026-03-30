namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrdersoundRelatedcoloumadd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderRequestProducts", "IsSound", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderRequestProducts", "IsAdmin", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderRequestServices", "IsSound", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderRequestServices", "IsAdmin", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderRequestServices", "IsAdmin");
            DropColumn("dbo.OrderRequestServices", "IsSound");
            DropColumn("dbo.OrderRequestProducts", "IsAdmin");
            DropColumn("dbo.OrderRequestProducts", "IsSound");
        }
    }
}
