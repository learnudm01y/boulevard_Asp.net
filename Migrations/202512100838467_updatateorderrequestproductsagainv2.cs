namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatateorderrequestproductsagainv2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderMasterStatusLogs",
                c => new
                    {
                        OrderMasterStatusLogId = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        CurrentInvoiceId = c.Int(nullable: false),
                        PriviousInvoiceId = c.Int(nullable: false),
                        DateTime = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrderMasterStatusLogId);
            
            AddColumn("dbo.OrderRequestProducts", "CourierOrderId", c => c.String(maxLength: 100));
            AddColumn("dbo.OrderRequestProducts", "CourierOrderResponse", c => c.String(maxLength: 500));
            AddColumn("dbo.OrderRequestProducts", "RiderName", c => c.String(maxLength: 100));
            AddColumn("dbo.OrderRequestProducts", "RiderPhone", c => c.String(maxLength: 100));
            AddColumn("dbo.OrderRequestProducts", "RiderPositionLat", c => c.String(maxLength: 100));
            AddColumn("dbo.OrderRequestProducts", "RiderPositionLong", c => c.String(maxLength: 100));
            AddColumn("dbo.OrderRequestProducts", "DeliveryImage", c => c.String(maxLength: 500));
            AddColumn("dbo.OrderRequestProducts", "IsCanceled", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderRequestProducts", "CancelReason", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderRequestProducts", "CancelReason");
            DropColumn("dbo.OrderRequestProducts", "IsCanceled");
            DropColumn("dbo.OrderRequestProducts", "DeliveryImage");
            DropColumn("dbo.OrderRequestProducts", "RiderPositionLong");
            DropColumn("dbo.OrderRequestProducts", "RiderPositionLat");
            DropColumn("dbo.OrderRequestProducts", "RiderPhone");
            DropColumn("dbo.OrderRequestProducts", "RiderName");
            DropColumn("dbo.OrderRequestProducts", "CourierOrderResponse");
            DropColumn("dbo.OrderRequestProducts", "CourierOrderId");
            DropTable("dbo.OrderMasterStatusLogs");
        }
    }
}
