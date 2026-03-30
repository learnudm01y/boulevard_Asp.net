namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addservicecategory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ServiceCategories",
                c => new
                    {
                        ServiceCategoryId = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        ServiceId = c.Int(),
                        Status = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.ServiceCategoryId)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Services", t => t.ServiceId)
                .Index(t => t.CategoryId)
                .Index(t => t.ServiceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceCategories", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.ServiceCategories", "CategoryId", "dbo.Categories");
            DropIndex("dbo.ServiceCategories", new[] { "ServiceId" });
            DropIndex("dbo.ServiceCategories", new[] { "CategoryId" });
            DropTable("dbo.ServiceCategories");
        }
    }
}
