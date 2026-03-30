namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateProductTypeaddressedupdateddv3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductTypeMasters", "PickUpContactNo", c => c.String(maxLength: 100));
            AddColumn("dbo.ProductTypeMasters", "PickUpContactName", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductTypeMasters", "PickUpContactName");
            DropColumn("dbo.ProductTypeMasters", "PickUpContactNo");
        }
    }
}
