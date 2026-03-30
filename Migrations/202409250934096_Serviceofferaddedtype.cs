namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Serviceofferaddedtype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceOffers", "ServiceTypeId", c => c.Int());
            CreateIndex("dbo.ServiceOffers", "ServiceTypeId");
            AddForeignKey("dbo.ServiceOffers", "ServiceTypeId", "dbo.ServiceTypes", "ServiceTypeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceOffers", "ServiceTypeId", "dbo.ServiceTypes");
            DropIndex("dbo.ServiceOffers", new[] { "ServiceTypeId" });
            DropColumn("dbo.ServiceOffers", "ServiceTypeId");
        }
    }
}
