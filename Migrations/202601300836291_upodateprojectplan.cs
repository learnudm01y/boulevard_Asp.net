namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class upodateprojectplan : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProjectPlans",
                c => new
                    {
                        ProjectPlanId = c.Int(nullable: false, identity: true),
                        ServiceTypeId = c.Int(),
                        Title = c.String(maxLength: 500),
                        UniteType = c.String(maxLength: 500),
                        Image = c.String(maxLength: 500),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ProjectPlanId)
                .ForeignKey("dbo.ServiceTypes", t => t.ServiceTypeId)
                .Index(t => t.ServiceTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectPlans", "ServiceTypeId", "dbo.ServiceTypes");
            DropIndex("dbo.ProjectPlans", new[] { "ServiceTypeId" });
            DropTable("dbo.ProjectPlans");
        }
    }
}
