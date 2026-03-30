namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateservicesCategoryaddtype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceCategories", "ServiceTypeId", c => c.Int());
            CreateIndex("dbo.ServiceCategories", "ServiceTypeId");
            AddForeignKey("dbo.ServiceCategories", "ServiceTypeId", "dbo.ServiceTypes", "ServiceTypeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceCategories", "ServiceTypeId", "dbo.ServiceTypes");
            DropIndex("dbo.ServiceCategories", new[] { "ServiceTypeId" });
            DropColumn("dbo.ServiceCategories", "ServiceTypeId");
        }
    }
}
