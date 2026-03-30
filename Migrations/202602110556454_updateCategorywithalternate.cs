namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateCategorywithalternate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "AlternateFeatureCategoryId", c => c.Int());
            AddColumn("dbo.Categories", "AlternateServiceId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Categories", "AlternateServiceId");
            DropColumn("dbo.Categories", "AlternateFeatureCategoryId");
        }
    }
}
