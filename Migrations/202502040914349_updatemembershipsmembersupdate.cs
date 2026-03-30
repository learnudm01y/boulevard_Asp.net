namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemembershipsmembersupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderRequestProducts", "IsMembershipOrder", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderRequestProducts", "MembershipDiscountType", c => c.String(maxLength: 100));
            AddColumn("dbo.OrderRequestProducts", "MembershipDiscountAmount", c => c.Double(nullable: false));
            AddColumn("dbo.OrderRequestProducts", "MembershipId", c => c.Int(nullable: false));
            AddColumn("dbo.OrderRequestServices", "IsMembershipOrder", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderRequestServices", "MembershipDiscountType", c => c.String(maxLength: 100));
            AddColumn("dbo.OrderRequestServices", "MembershipDiscountAmount", c => c.Double(nullable: false));
            AddColumn("dbo.OrderRequestServices", "MembershipId", c => c.Int(nullable: false));
            AlterColumn("dbo.MemberShipDiscountCategories", "MemberShipDiscountAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MemberShipDiscountCategories", "MemberShipDiscountAmount", c => c.String(maxLength: 100));
            DropColumn("dbo.OrderRequestServices", "MembershipId");
            DropColumn("dbo.OrderRequestServices", "MembershipDiscountAmount");
            DropColumn("dbo.OrderRequestServices", "MembershipDiscountType");
            DropColumn("dbo.OrderRequestServices", "IsMembershipOrder");
            DropColumn("dbo.OrderRequestProducts", "MembershipId");
            DropColumn("dbo.OrderRequestProducts", "MembershipDiscountAmount");
            DropColumn("dbo.OrderRequestProducts", "MembershipDiscountType");
            DropColumn("dbo.OrderRequestProducts", "IsMembershipOrder");
        }
    }
}
