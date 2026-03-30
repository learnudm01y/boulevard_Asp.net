namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateserviceorder : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderRequestServices", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.OrderRequestServices", "ServiceTypeId", "dbo.ServiceTypes");
            DropIndex("dbo.OrderRequestServices", new[] { "ServiceId" });
            DropIndex("dbo.OrderRequestServices", new[] { "ServiceTypeId" });
            CreateTable(
                "dbo.OrderRequestServiceDetails",
                c => new
                    {
                        OrderRequestServiceDetailsId = c.Int(nullable: false, identity: true),
                        OrderRequestServiceId = c.Int(nullable: false),
                        ServiceId = c.Int(),
                        ServiceTypeId = c.Int(),
                        GrossPrice = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.OrderRequestServiceDetailsId)
                .ForeignKey("dbo.OrderRequestServices", t => t.OrderRequestServiceId, cascadeDelete: true)
                .ForeignKey("dbo.Services", t => t.ServiceId)
                .ForeignKey("dbo.ServiceTypes", t => t.ServiceTypeId)
                .Index(t => t.OrderRequestServiceId)
                .Index(t => t.ServiceId)
                .Index(t => t.ServiceTypeId);
            
            CreateTable(
                "dbo.ServiceLandmarks",
                c => new
                    {
                        ServiceLandmarkId = c.Int(nullable: false, identity: true),
                        ServiceLandmarkKey = c.Guid(nullable: false),
                        Name = c.String(),
                        Address = c.String(),
                        Distance = c.Double(nullable: false),
                        Latitude = c.String(),
                        Longitude = c.String(),
                        ServiceId = c.Int(nullable: false),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ServiceLandmarkId)
                .ForeignKey("dbo.Services", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.ServiceId);
            
            AddColumn("dbo.OrderRequestServices", "TotalPrice", c => c.Double(nullable: false));
            DropColumn("dbo.OrderRequestServices", "ServiceId");
            DropColumn("dbo.OrderRequestServices", "ServiceTypeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderRequestServices", "ServiceTypeId", c => c.Int());
            AddColumn("dbo.OrderRequestServices", "ServiceId", c => c.Int(nullable: false));
            DropForeignKey("dbo.OrderRequestServiceDetails", "ServiceTypeId", "dbo.ServiceTypes");
            DropForeignKey("dbo.OrderRequestServiceDetails", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.ServiceLandmarks", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.OrderRequestServiceDetails", "OrderRequestServiceId", "dbo.OrderRequestServices");
            DropIndex("dbo.ServiceLandmarks", new[] { "ServiceId" });
            DropIndex("dbo.OrderRequestServiceDetails", new[] { "ServiceTypeId" });
            DropIndex("dbo.OrderRequestServiceDetails", new[] { "ServiceId" });
            DropIndex("dbo.OrderRequestServiceDetails", new[] { "OrderRequestServiceId" });
            DropColumn("dbo.OrderRequestServices", "TotalPrice");
            DropTable("dbo.ServiceLandmarks");
            DropTable("dbo.OrderRequestServiceDetails");
            CreateIndex("dbo.OrderRequestServices", "ServiceTypeId");
            CreateIndex("dbo.OrderRequestServices", "ServiceId");
            AddForeignKey("dbo.OrderRequestServices", "ServiceTypeId", "dbo.ServiceTypes", "ServiceTypeId");
            AddForeignKey("dbo.OrderRequestServices", "ServiceId", "dbo.Services", "ServiceId", cascadeDelete: true);
        }
    }
}
