namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateproducttypeinproduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "ProductType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "ProductType");
        }
    }
}
