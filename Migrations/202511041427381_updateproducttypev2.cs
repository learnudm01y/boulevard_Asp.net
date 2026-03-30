namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateproducttypev2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductTypeMasters",
                c => new
                    {
                        ProductTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        NameAr = c.String(maxLength: 100),
                        Description = c.String(maxLength: 200),
                        DescriptionAr = c.String(maxLength: 200),
                        DeliveryTime = c.String(maxLength: 100),
                        Status = c.String(maxLength: 10),
                    })
                .PrimaryKey(t => t.ProductTypeId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ProductTypeMasters");
        }
    }
}
