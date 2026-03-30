namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateAdminNotifications : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdminNotifications",
                c => new
                    {
                        AdminNotificationId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Title = c.String(maxLength: 100),
                        Message = c.String(),
                        FeatureType = c.String(maxLength: 100),
                        UserType = c.String(maxLength: 100),
                        IsSent = c.Boolean(nullable: false),
                        IsReceived = c.Boolean(nullable: false),
                        IsSeen = c.Boolean(nullable: false),
                        SentBy = c.Int(nullable: false),
                        ReceivedBy = c.Int(nullable: false),
                        SeenBy = c.Int(nullable: false),
                        SentAt = c.DateTime(),
                        ReceivedAt = c.DateTime(),
                        SeenAt = c.DateTime(),
                        Status = c.Boolean(nullable: false),
                        LastModified = c.DateTime(nullable: false),
                        SellerId = c.Int(nullable: false),
                        OrderId = c.Int(),
                    })
                .PrimaryKey(t => t.AdminNotificationId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AdminNotifications");
        }
    }
}
