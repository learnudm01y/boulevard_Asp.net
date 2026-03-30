namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemembershiptable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MemberShipDiscountCategories",
                c => new
                    {
                        MemberShipDiscountCategoryId = c.Int(nullable: false, identity: true),
                        MemberShipId = c.Int(nullable: false),
                        FeatureCategoryId = c.Int(nullable: false),
                        MemberShipDiscountType = c.String(maxLength: 100),
                        MemberShipDiscountAmount = c.String(maxLength: 100),
                        UpdateAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MemberShipDiscountCategoryId)
                .ForeignKey("dbo.FeatureCategories", t => t.FeatureCategoryId, cascadeDelete: true)
                .Index(t => t.FeatureCategoryId);
            
            CreateTable(
                "dbo.MemberShips",
                c => new
                    {
                        MemberShipId = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 250),
                        Description = c.String(maxLength: 500),
                        MembershipValidityInMonth = c.Int(nullable: false),
                        MemberShipDiscountType = c.String(maxLength: 100),
                        MemberShipDiscountAmount = c.String(maxLength: 100),
                        Benefits = c.String(),
                        MembershipBanner = c.String(maxLength: 250),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.MemberShipId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MemberShipDiscountCategories", "FeatureCategoryId", "dbo.FeatureCategories");
            DropIndex("dbo.MemberShipDiscountCategories", new[] { "FeatureCategoryId" });
            DropTable("dbo.MemberShips");
            DropTable("dbo.MemberShipDiscountCategories");
        }
    }
}
