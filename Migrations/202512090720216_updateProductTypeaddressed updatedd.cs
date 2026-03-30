namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateProductTypeaddressedupdatedd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductTypeMasters", "Pickup_Building", c => c.String(maxLength: 500));
            AddColumn("dbo.ProductTypeMasters", "Pickup_Street", c => c.String(maxLength: 500));
            AddColumn("dbo.ProductTypeMasters", "Pickup_Area", c => c.String(maxLength: 500));
            AddColumn("dbo.ProductTypeMasters", "Pickup_City", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductTypeMasters", "Pickup_City");
            DropColumn("dbo.ProductTypeMasters", "Pickup_Area");
            DropColumn("dbo.ProductTypeMasters", "Pickup_Street");
            DropColumn("dbo.ProductTypeMasters", "Pickup_Building");
        }
    }
}
