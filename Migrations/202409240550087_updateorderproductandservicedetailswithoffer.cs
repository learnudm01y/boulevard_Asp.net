namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateorderproductandservicedetailswithoffer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderRequestProductDetails", "OfferInformationId", c => c.Int());
            AddColumn("dbo.OrderRequestServiceDetails", "OfferInformationId", c => c.Int());
            CreateIndex("dbo.OrderRequestProductDetails", "OfferInformationId");
            CreateIndex("dbo.OrderRequestServiceDetails", "OfferInformationId");
            AddForeignKey("dbo.OrderRequestProductDetails", "OfferInformationId", "dbo.OfferInformations", "OfferInformationId");
            AddForeignKey("dbo.OrderRequestServiceDetails", "OfferInformationId", "dbo.OfferInformations", "OfferInformationId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderRequestServiceDetails", "OfferInformationId", "dbo.OfferInformations");
            DropForeignKey("dbo.OrderRequestProductDetails", "OfferInformationId", "dbo.OfferInformations");
            DropIndex("dbo.OrderRequestServiceDetails", new[] { "OfferInformationId" });
            DropIndex("dbo.OrderRequestProductDetails", new[] { "OfferInformationId" });
            DropColumn("dbo.OrderRequestServiceDetails", "OfferInformationId");
            DropColumn("dbo.OrderRequestProductDetails", "OfferInformationId");
        }
    }
}
