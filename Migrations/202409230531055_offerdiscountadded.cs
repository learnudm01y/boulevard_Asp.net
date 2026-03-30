namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class offerdiscountadded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OfferDiscounts",
                c => new
                    {
                        OfferDiscountId = c.Int(nullable: false, identity: true),
                        DiscountType = c.String(maxLength: 100),
                        DiscountAmount = c.Int(nullable: false),
                        OfferInformationId = c.Int(),
                    })
                .PrimaryKey(t => t.OfferDiscountId)
                .ForeignKey("dbo.OfferInformations", t => t.OfferInformationId)
                .Index(t => t.OfferInformationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OfferDiscounts", "OfferInformationId", "dbo.OfferInformations");
            DropIndex("dbo.OfferDiscounts", new[] { "OfferInformationId" });
            DropTable("dbo.OfferDiscounts");
        }
    }
}
