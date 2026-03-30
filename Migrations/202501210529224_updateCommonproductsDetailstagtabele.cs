namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateCommonproductsDetailstagtabele : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CommonProductTagDetails",
                c => new
                    {
                        CommonProductTagDetailsId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        CommonProductTagId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CommonProductTagDetailsId);
            
            AlterColumn("dbo.CommonProductTags", "Status", c => c.String(maxLength: 100));
            DropColumn("dbo.CommonProductTags", "FeatureTypeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CommonProductTags", "FeatureTypeId", c => c.Int());
            AlterColumn("dbo.CommonProductTags", "Status", c => c.String(maxLength: 10));
            DropTable("dbo.CommonProductTagDetails");
        }
    }
}
