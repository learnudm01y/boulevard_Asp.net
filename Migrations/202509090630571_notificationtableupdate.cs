namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notificationtableupdate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        NotificationId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Title = c.String(),
                        Message = c.String(),
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
                    })
                .PrimaryKey(t => t.NotificationId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Notifications");
        }
    }
}
