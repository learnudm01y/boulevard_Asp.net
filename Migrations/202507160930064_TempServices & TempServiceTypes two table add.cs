namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TempServicesTempServiceTypestwotableadd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TempServices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SlNo = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        ServiceHour = c.String(),
                        Category = c.String(),
                        SubCategory = c.String(),
                        Address = c.String(),
                        AboutUs = c.String(),
                        Languages = c.String(),
                        ScopeofService = c.String(),
                        ServiceMinutes = c.String(),
                        Price = c.String(),
                        ProductImages = c.String(),
                        File = c.String(),
                        ExcelCount = c.Int(nullable: false),
                        FeatureCategoryId = c.Int(nullable: false),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TempServiceTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SlNo = c.String(),
                        ServiceName = c.String(),
                        ServiceTypeName = c.String(),
                        PersoneQuantity = c.String(),
                        Description = c.String(),
                        Size = c.String(),
                        Images = c.String(),
                        Price = c.String(),
                        AdultQuantity = c.String(),
                        ChildrenQuantity = c.String(),
                        PaymentType = c.String(),
                        ServiceHour = c.String(),
                        ServiceMinutes = c.String(),
                        FileType = c.String(),
                        File = c.String(),
                        AmenitiesName = c.String(),
                        AmenitiesLogo = c.String(),
                        AmenitiesType = c.String(),
                        ExcelCount = c.Int(nullable: false),
                        FeatureCategoryId = c.Int(nullable: false),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TempServiceTypes");
            DropTable("dbo.TempServices");
        }
    }
}
