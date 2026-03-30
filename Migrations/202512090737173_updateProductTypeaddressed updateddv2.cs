namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateProductTypeaddressedupdateddv2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductTypeMasters", "Latitute", c => c.String(maxLength: 100));
            AddColumn("dbo.ProductTypeMasters", "Longitute", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductTypeMasters", "Longitute");
            DropColumn("dbo.ProductTypeMasters", "Latitute");
        }
    }
}
