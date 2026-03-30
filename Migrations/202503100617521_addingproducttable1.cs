namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingproducttable1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Barcode", c => c.String());
            AddColumn("dbo.Products", "StockQuantity", c => c.Int(nullable: false));
            DropColumn("dbo.Airports", "IsSelected");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Airports", "IsSelected", c => c.Boolean(nullable: false));
            DropColumn("dbo.Products", "StockQuantity");
            DropColumn("dbo.Products", "Barcode");
        }
    }
}
