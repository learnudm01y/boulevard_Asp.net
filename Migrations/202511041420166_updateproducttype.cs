namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateproducttype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FeatureCategories", "DeliveryCharge", c => c.Double());
            AddColumn("dbo.FeatureCategories", "ServiceFee", c => c.Double());
            AddColumn("dbo.OrderRequestProducts", "ServiceCharge", c => c.Double(nullable: false));
            AddColumn("dbo.OrderRequestProducts", "Tip", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderRequestProducts", "Tip");
            DropColumn("dbo.OrderRequestProducts", "ServiceCharge");
            DropColumn("dbo.FeatureCategories", "ServiceFee");
            DropColumn("dbo.FeatureCategories", "DeliveryCharge");
        }
    }
}
