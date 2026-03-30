namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemembershiptablev3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MemberShips", "MemberShipKey", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MemberShips", "MemberShipKey");
        }
    }
}
