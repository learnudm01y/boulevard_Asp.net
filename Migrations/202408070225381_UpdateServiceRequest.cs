namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateServiceRequest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderRequestServices",
                c => new
                    {
                        OrderRequestServiceId = c.Int(nullable: false, identity: true),
                        BookingId = c.String(maxLength: 100),
                        BookingMemberType = c.String(maxLength: 100),
                        MemberId = c.Long(nullable: false),
                        ServiceId = c.Int(nullable: false),
                        ServiceTypeId = c.Int(),
                        MemberNameTitle = c.String(maxLength: 10),
                        FirstName = c.String(maxLength: 100),
                        LastName = c.String(maxLength: 100),
                        Email = c.String(maxLength: 100),
                        PhoneCode = c.String(maxLength: 50),
                        PhoneNo = c.String(maxLength: 100),
                        ExtraCharge = c.Double(nullable: false),
                        BookingDate = c.DateTime(nullable: false),
                        BookingStatus = c.String(),
                    })
                .PrimaryKey(t => t.OrderRequestServiceId)
                .ForeignKey("dbo.Members", t => t.MemberId, cascadeDelete: true)
                .ForeignKey("dbo.Services", t => t.ServiceId, cascadeDelete: true)
                .ForeignKey("dbo.ServiceTypes", t => t.ServiceTypeId)
                .Index(t => t.MemberId)
                .Index(t => t.ServiceId)
                .Index(t => t.ServiceTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderRequestServices", "ServiceTypeId", "dbo.ServiceTypes");
            DropForeignKey("dbo.OrderRequestServices", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.OrderRequestServices", "MemberId", "dbo.Members");
            DropIndex("dbo.OrderRequestServices", new[] { "ServiceTypeId" });
            DropIndex("dbo.OrderRequestServices", new[] { "ServiceId" });
            DropIndex("dbo.OrderRequestServices", new[] { "MemberId" });
            DropTable("dbo.OrderRequestServices");
        }
    }
}
