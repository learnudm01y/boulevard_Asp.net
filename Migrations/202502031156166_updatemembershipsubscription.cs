namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemembershipsubscription : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MemberSubscriptions",
                c => new
                    {
                        MemberSubscriptionId = c.Int(nullable: false, identity: true),
                        MemberShipId = c.Int(nullable: false),
                        MemberId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Status = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MemberSubscriptionId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MemberSubscriptions");
        }
    }
}
