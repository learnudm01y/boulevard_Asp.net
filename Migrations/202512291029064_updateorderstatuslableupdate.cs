namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateorderstatuslableupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderStatus", "IsPublic", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderStatus", "IsInternal", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderStatus", "Label", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderStatus", "Label");
            DropColumn("dbo.OrderStatus", "IsInternal");
            DropColumn("dbo.OrderStatus", "IsPublic");
        }
    }
}
