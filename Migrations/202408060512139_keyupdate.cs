namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class keyupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceAmenities", "ServiceAmenityKey", c => c.Guid(nullable: false));
            AddColumn("dbo.Services", "ServiceKey", c => c.Guid(nullable: false));
            AddColumn("dbo.ServiceTypes", "ServiceTypeKey", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceTypes", "ServiceTypeKey");
            DropColumn("dbo.Services", "ServiceKey");
            DropColumn("dbo.ServiceAmenities", "ServiceAmenityKey");
        }
    }
}
