namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatememberOrdersystem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderRequestProductDetails", "IsMembershipOrder", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderRequestProductDetails", "DiscountType", c => c.String(maxLength: 100));
            AddColumn("dbo.OrderRequestProductDetails", "DiscountAmount", c => c.Double(nullable: false));
            AddColumn("dbo.OrderRequestProductDetails", "MembershipId", c => c.Int(nullable: false));
            AddColumn("dbo.OrderRequestServiceDetails", "IsMembershipOrder", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderRequestServiceDetails", "DiscountType", c => c.String(maxLength: 100));
            AddColumn("dbo.OrderRequestServiceDetails", "DiscountAmount", c => c.Double(nullable: false));
            AddColumn("dbo.OrderRequestServiceDetails", "MembershipId", c => c.Int(nullable: false));
            DropColumn("dbo.OrderRequestProducts", "IsMembershipOrder");
            DropColumn("dbo.OrderRequestProducts", "MembershipDiscountType");
            DropColumn("dbo.OrderRequestProducts", "MembershipDiscountAmount");
            DropColumn("dbo.OrderRequestProducts", "MembershipId");
            DropColumn("dbo.OrderRequestServices", "IsMembershipOrder");
            DropColumn("dbo.OrderRequestServices", "MembershipDiscountType");
            DropColumn("dbo.OrderRequestServices", "MembershipDiscountAmount");
            DropColumn("dbo.OrderRequestServices", "MembershipId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderRequestServices", "MembershipId", c => c.Int(nullable: false));
            AddColumn("dbo.OrderRequestServices", "MembershipDiscountAmount", c => c.Double(nullable: false));
            AddColumn("dbo.OrderRequestServices", "MembershipDiscountType", c => c.String(maxLength: 100));
            AddColumn("dbo.OrderRequestServices", "IsMembershipOrder", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderRequestProducts", "MembershipId", c => c.Int(nullable: false));
            AddColumn("dbo.OrderRequestProducts", "MembershipDiscountAmount", c => c.Double(nullable: false));
            AddColumn("dbo.OrderRequestProducts", "MembershipDiscountType", c => c.String(maxLength: 100));
            AddColumn("dbo.OrderRequestProducts", "IsMembershipOrder", c => c.Boolean(nullable: false));
            DropColumn("dbo.OrderRequestServiceDetails", "MembershipId");
            DropColumn("dbo.OrderRequestServiceDetails", "DiscountAmount");
            DropColumn("dbo.OrderRequestServiceDetails", "DiscountType");
            DropColumn("dbo.OrderRequestServiceDetails", "IsMembershipOrder");
            DropColumn("dbo.OrderRequestProductDetails", "MembershipId");
            DropColumn("dbo.OrderRequestProductDetails", "DiscountAmount");
            DropColumn("dbo.OrderRequestProductDetails", "DiscountType");
            DropColumn("dbo.OrderRequestProductDetails", "IsMembershipOrder");
        }
    }
}
