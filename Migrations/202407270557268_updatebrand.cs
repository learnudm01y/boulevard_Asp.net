namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatebrand : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Brands",
                c => new
                    {
                        BrandId = c.Int(nullable: false, identity: true),
                        BrandKey = c.Guid(nullable: false),
                        Title = c.String(maxLength: 250),
                        Details = c.String(),
                        Image = c.String(),
                        FeatureCategoryId = c.Int(nullable: false),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.BrandId)
                .ForeignKey("dbo.FeatureCategories", t => t.FeatureCategoryId, cascadeDelete: true)
                .Index(t => t.FeatureCategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Brands", "FeatureCategoryId", "dbo.FeatureCategories");
            DropIndex("dbo.Brands", new[] { "FeatureCategoryId" });
            DropTable("dbo.Brands");
        }
    }
}
