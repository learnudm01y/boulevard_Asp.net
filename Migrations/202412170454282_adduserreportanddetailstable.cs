namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adduserreportanddetailstable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserReportDetails",
                c => new
                    {
                        UserReportDetailsId = c.Int(nullable: false, identity: true),
                        UserReportId = c.Int(nullable: false),
                        Response = c.String(),
                        LastUpdate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserReportDetailsId);
            
            CreateTable(
                "dbo.UserReports",
                c => new
                    {
                        UserReportId = c.Int(nullable: false, identity: true),
                        MemberId = c.Int(nullable: false),
                        Title = c.String(),
                        Comments = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        LastUpdate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserReportId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserReports");
            DropTable("dbo.UserReportDetails");
        }
    }
}
