namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TempProductFeatureCategoryIdcoloumadd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TempProducts", "FeatureCategoryId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TempProducts", "FeatureCategoryId");
        }
    }
}
