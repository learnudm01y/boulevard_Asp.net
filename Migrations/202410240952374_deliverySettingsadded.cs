namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deliverySettingsadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DeliverySettings",
                c => new
                    {
                        DeliverySettingId = c.Int(nullable: false, identity: true),
                        SettingKey = c.Guid(nullable: false),
                        DeliveryCharge = c.Int(nullable: false),
                        ChargeForFree = c.Double(nullable: false),
                        Status = c.String(maxLength: 100),
                        FeatureCategoryId = c.Int(nullable: false),
                        UpdateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DeliverySettingId)
                .ForeignKey("dbo.FeatureCategories", t => t.FeatureCategoryId, cascadeDelete: true)
                .Index(t => t.FeatureCategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DeliverySettings", "FeatureCategoryId", "dbo.FeatureCategories");
            DropIndex("dbo.DeliverySettings", new[] { "FeatureCategoryId" });
            DropTable("dbo.DeliverySettings");
        }
    }
}
