namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecartfeaturecat : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Carts", "FeatureCategoryId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Carts", "FeatureCategoryId");
        }
    }
}
