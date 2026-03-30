namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateenquerytable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerEnqueries",
                c => new
                    {
                        CustomerEnqueryId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 250),
                        Email = c.String(maxLength: 250),
                        Message = c.String(),
                        PhoneCode = c.String(maxLength: 50),
                        Phonenumber = c.String(maxLength: 100),
                        FeatureCategoryId = c.Int(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                        Status = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.CustomerEnqueryId)
                .ForeignKey("dbo.FeatureCategories", t => t.FeatureCategoryId, cascadeDelete: true)
                .Index(t => t.FeatureCategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerEnqueries", "FeatureCategoryId", "dbo.FeatureCategories");
            DropIndex("dbo.CustomerEnqueries", new[] { "FeatureCategoryId" });
            DropTable("dbo.CustomerEnqueries");
        }
    }
}
