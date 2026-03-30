namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateOrderstatusv2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderRequestProducts", "OrderStatusId", c => c.Int());
            CreateIndex("dbo.OrderRequestProducts", "OrderStatusId");
            AddForeignKey("dbo.OrderRequestProducts", "OrderStatusId", "dbo.OrderStatus", "OrderStatusId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderRequestProducts", "OrderStatusId", "dbo.OrderStatus");
            DropIndex("dbo.OrderRequestProducts", new[] { "OrderStatusId" });
            DropColumn("dbo.OrderRequestProducts", "OrderStatusId");
        }
    }
}
