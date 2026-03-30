namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemembershipsubscriptionforiegnkey : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.MemberSubscriptions", "MemberShipId");
            AddForeignKey("dbo.MemberSubscriptions", "MemberShipId", "dbo.MemberShips", "MemberShipId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MemberSubscriptions", "MemberShipId", "dbo.MemberShips");
            DropIndex("dbo.MemberSubscriptions", new[] { "MemberShipId" });
        }
    }
}
