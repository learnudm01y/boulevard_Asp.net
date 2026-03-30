namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatesometables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderRequestProductDetails",
                c => new
                    {
                        OrderRequestProductDetailsId = c.Int(nullable: false, identity: true),
                        OrderRequestProductId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        GrossPrice = c.Double(nullable: false),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.OrderRequestProductDetailsId)
                .ForeignKey("dbo.OrderRequestProducts", t => t.OrderRequestProductId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.OrderRequestProductId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.OrderRequestProducts",
                c => new
                    {
                        OrderRequestProductId = c.Int(nullable: false, identity: true),
                        ReadableOrderId = c.String(),
                        MemberId = c.Long(nullable: false),
                        MemberAddressId = c.Long(),
                        OrderDateTime = c.DateTime(nullable: false),
                        DeliveryDateTime = c.DateTime(nullable: false),
                        Comments = c.String(maxLength: 500),
                        DeliveryCharge = c.Double(nullable: false),
                        TotalPrice = c.Double(nullable: false),
                        PaymentMethodId = c.Int(nullable: false),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.OrderRequestProductId)
                .ForeignKey("dbo.Members", t => t.MemberId, cascadeDelete: true)
                .ForeignKey("dbo.MemberAddresses", t => t.MemberAddressId)
                .ForeignKey("dbo.PaymentMethods", t => t.PaymentMethodId, cascadeDelete: true)
                .Index(t => t.MemberId)
                .Index(t => t.MemberAddressId)
                .Index(t => t.PaymentMethodId);
            
            CreateTable(
                "dbo.PaymentMethods",
                c => new
                    {
                        PaymentMethodId = c.Int(nullable: false, identity: true),
                        PaymentMethodKey = c.Guid(nullable: false),
                        PaymentMethodName = c.String(),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.PaymentMethodId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderRequestProductDetails", "ProductId", "dbo.Products");
            DropForeignKey("dbo.OrderRequestProductDetails", "OrderRequestProductId", "dbo.OrderRequestProducts");
            DropForeignKey("dbo.OrderRequestProducts", "PaymentMethodId", "dbo.PaymentMethods");
            DropForeignKey("dbo.OrderRequestProducts", "MemberAddressId", "dbo.MemberAddresses");
            DropForeignKey("dbo.OrderRequestProducts", "MemberId", "dbo.Members");
            DropIndex("dbo.OrderRequestProducts", new[] { "PaymentMethodId" });
            DropIndex("dbo.OrderRequestProducts", new[] { "MemberAddressId" });
            DropIndex("dbo.OrderRequestProducts", new[] { "MemberId" });
            DropIndex("dbo.OrderRequestProductDetails", new[] { "ProductId" });
            DropIndex("dbo.OrderRequestProductDetails", new[] { "OrderRequestProductId" });
            DropTable("dbo.PaymentMethods");
            DropTable("dbo.OrderRequestProducts");
            DropTable("dbo.OrderRequestProductDetails");
        }
    }
}
