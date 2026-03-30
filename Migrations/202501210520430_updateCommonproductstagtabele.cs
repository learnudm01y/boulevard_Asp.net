namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateCommonproductstagtabele : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CommonProductTags",
                c => new
                    {
                        CommonProductTagId = c.Int(nullable: false, identity: true),
                        TagName = c.String(maxLength: 250),
                        FeatureCategoryId = c.Int(),
                        FeatureTypeId = c.Int(),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.CommonProductTagId)
                .ForeignKey("dbo.FeatureCategories", t => t.FeatureCategoryId)
                .Index(t => t.FeatureCategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CommonProductTags", "FeatureCategoryId", "dbo.FeatureCategories");
            DropIndex("dbo.CommonProductTags", new[] { "FeatureCategoryId" });
            DropTable("dbo.CommonProductTags");
        }
    }
}
