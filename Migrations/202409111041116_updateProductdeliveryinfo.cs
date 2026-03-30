namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateProductdeliveryinfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "AvgRatings", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "DeliveryInfo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "DeliveryInfo");
            DropColumn("dbo.Products", "AvgRatings");
        }
    }
}
