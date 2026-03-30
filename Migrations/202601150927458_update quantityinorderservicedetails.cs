namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatequantityinorderservicedetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderRequestServiceDetails", "Quantity", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderRequestServiceDetails", "Quantity");
        }
    }
}
