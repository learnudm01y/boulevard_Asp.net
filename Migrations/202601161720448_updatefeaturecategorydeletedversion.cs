namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatefeaturecategorydeletedversion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FeatureCategories", "IsDelete", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FeatureCategories", "IsDelete");
        }
    }
}
