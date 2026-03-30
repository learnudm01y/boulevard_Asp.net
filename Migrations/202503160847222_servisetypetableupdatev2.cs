namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class servisetypetableupdatev2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceTypeFiles", "ServiceAmenityId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceTypeFiles", "ServiceAmenityId");
        }
    }
}
