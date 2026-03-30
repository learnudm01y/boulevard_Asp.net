namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class teotableaddtwotablecoloumadd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GolbalMemberCategories",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MemberId = c.Long(nullable: false),
                        FeatureCategoryId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MonthlyGoals",
                c => new
                    {
                        MonthlyGoalId = c.Int(nullable: false, identity: true),
                        MonthlyGoalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.MonthlyGoalId);
            
            AddColumn("dbo.Members", "OTPGenerateDateTime", c => c.DateTime());
            AddColumn("dbo.Members", "MonthlyGoalId", c => c.Int(nullable: false));
            AddColumn("dbo.Members", "MonthlyGoalAmount", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Products", "IsScheduled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "IsScheduled");
            DropColumn("dbo.Members", "MonthlyGoalAmount");
            DropColumn("dbo.Members", "MonthlyGoalId");
            DropColumn("dbo.Members", "OTPGenerateDateTime");
            DropTable("dbo.MonthlyGoals");
            DropTable("dbo.GolbalMemberCategories");
        }
    }
}
