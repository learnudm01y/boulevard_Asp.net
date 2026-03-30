namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemembershiptablev2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.MemberShips", "MemberShipDiscountType");
            DropColumn("dbo.MemberShips", "MemberShipDiscountAmount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MemberShips", "MemberShipDiscountAmount", c => c.String(maxLength: 100));
            AddColumn("dbo.MemberShips", "MemberShipDiscountType", c => c.String(maxLength: 100));
        }
    }
}
