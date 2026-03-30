namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TempProducttableProductNamecoloumadd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TempProducts", "ProductName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TempProducts", "ProductName");
        }
    }
}
