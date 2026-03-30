namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatefeatureandOrderrequestTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FeatureCategories", "IsWaitForApproval", c => c.Boolean(nullable: false));
            AddColumn("dbo.FeatureCategories", "IsQuoteEnable", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderRequestProducts", "PaymentStatus", c => c.String(maxLength: 100));
            AddColumn("dbo.OrderRequestProducts", "PaymentTransectionId", c => c.String(maxLength: 100));
            AddColumn("dbo.OrderRequestServices", "PaymentStatus", c => c.String(maxLength: 100));
            AddColumn("dbo.OrderRequestServices", "QuotedPrice", c => c.Double(nullable: false));
            AddColumn("dbo.OrderRequestServices", "QuotationNote", c => c.String());
            AddColumn("dbo.OrderRequestServices", "QuotationFileLink", c => c.String(maxLength: 200));
            AddColumn("dbo.OrderRequestServices", "IsApprovedByAdmin", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderRequestServices", "ApprovedBy", c => c.Int(nullable: false));
            AddColumn("dbo.OrderRequestServices", "PaymentTransectionId", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderRequestServices", "PaymentTransectionId");
            DropColumn("dbo.OrderRequestServices", "ApprovedBy");
            DropColumn("dbo.OrderRequestServices", "IsApprovedByAdmin");
            DropColumn("dbo.OrderRequestServices", "QuotationFileLink");
            DropColumn("dbo.OrderRequestServices", "QuotationNote");
            DropColumn("dbo.OrderRequestServices", "QuotedPrice");
            DropColumn("dbo.OrderRequestServices", "PaymentStatus");
            DropColumn("dbo.OrderRequestProducts", "PaymentTransectionId");
            DropColumn("dbo.OrderRequestProducts", "PaymentStatus");
            DropColumn("dbo.FeatureCategories", "IsQuoteEnable");
            DropColumn("dbo.FeatureCategories", "IsWaitForApproval");
        }
    }
}
