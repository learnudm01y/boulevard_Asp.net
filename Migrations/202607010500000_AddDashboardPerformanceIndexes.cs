namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDashboardPerformanceIndexes : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.OrderRequestProducts", "OrderDateTime", name: "IX_OrderRequestProducts_OrderDateTime");
            CreateIndex("dbo.OrderRequestProducts", "DeliveryDateTime", name: "IX_OrderRequestProducts_DeliveryDateTime");
            CreateIndex("dbo.OrderRequestProducts", "CreateDate", name: "IX_OrderRequestProducts_CreateDate");
            CreateIndex("dbo.OrderRequestProducts", "FeatureCategoryId", name: "IX_OrderRequestProducts_FeatureCategoryId");
            CreateIndex("dbo.OrderRequestServices", "BookingDate", name: "IX_OrderRequestServices_BookingDate");
            CreateIndex("dbo.OrderRequestServices", "FeatureCategoryId", name: "IX_OrderRequestServices_FeatureCategoryId");
            CreateIndex("dbo.Members", "Status", name: "IX_Members_Status");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Members", "IX_Members_Status");
            DropIndex("dbo.OrderRequestServices", "IX_OrderRequestServices_FeatureCategoryId");
            DropIndex("dbo.OrderRequestServices", "IX_OrderRequestServices_BookingDate");
            DropIndex("dbo.OrderRequestProducts", "IX_OrderRequestProducts_FeatureCategoryId");
            DropIndex("dbo.OrderRequestProducts", "IX_OrderRequestProducts_CreateDate");
            DropIndex("dbo.OrderRequestProducts", "IX_OrderRequestProducts_DeliveryDateTime");
            DropIndex("dbo.OrderRequestProducts", "IX_OrderRequestProducts_OrderDateTime");
        }
    }
}
