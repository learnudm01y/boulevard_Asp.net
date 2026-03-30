namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatebrandandwebhtml : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Brands", "MediumImage", c => c.String());
            AddColumn("dbo.Brands", "LargeImage", c => c.String());
            AddColumn("dbo.Brands", "IsFeature", c => c.Boolean());
            AddColumn("dbo.Brands", "IsTrenbding", c => c.Boolean());
            AddColumn("dbo.WebHtmls", "BrandId", c => c.Int(nullable: false));
            AddColumn("dbo.WebHtmls", "CategoryId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WebHtmls", "CategoryId");
            DropColumn("dbo.WebHtmls", "BrandId");
            DropColumn("dbo.Brands", "IsTrenbding");
            DropColumn("dbo.Brands", "IsFeature");
            DropColumn("dbo.Brands", "LargeImage");
            DropColumn("dbo.Brands", "MediumImage");
        }
    }
}
