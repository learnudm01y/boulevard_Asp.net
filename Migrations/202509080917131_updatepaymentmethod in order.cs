namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatepaymentmethodinorder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderRequestServices", "PaymentMethodId", c => c.Int());
            CreateIndex("dbo.OrderRequestServices", "PaymentMethodId");
            AddForeignKey("dbo.OrderRequestServices", "PaymentMethodId", "dbo.PaymentMethods", "PaymentMethodId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderRequestServices", "PaymentMethodId", "dbo.PaymentMethods");
            DropIndex("dbo.OrderRequestServices", new[] { "PaymentMethodId" });
            DropColumn("dbo.OrderRequestServices", "PaymentMethodId");
        }
    }
}
