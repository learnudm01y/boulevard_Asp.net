namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateservicecart : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CartServices",
                c => new
                    {
                        CartServiceId = c.Int(nullable: false, identity: true),
                        MemberId = c.Int(nullable: false),
                        ServiceId = c.Int(nullable: false),
                        ServiceTypeId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        FeatureCategoryId = c.Int(nullable: false),
                        Status = c.String(maxLength: 50),
                        LastModified = c.DateTime(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CartServiceId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CartServices");
        }
    }
}
