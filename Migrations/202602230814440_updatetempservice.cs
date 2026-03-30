namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetempservice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TempServices", "SubServiceTypeName", c => c.String(maxLength: 250));
            AddColumn("dbo.TempServices", "SubServiceTypeNameAr", c => c.String(maxLength: 250));
            AddColumn("dbo.TempServices", "ServiceTypePrice", c => c.String());
            AddColumn("dbo.TempServices", "ServiceTypePriceAr", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TempServices", "ServiceTypePriceAr");
            DropColumn("dbo.TempServices", "ServiceTypePrice");
            DropColumn("dbo.TempServices", "SubServiceTypeNameAr");
            DropColumn("dbo.TempServices", "SubServiceTypeName");
        }
    }
}
