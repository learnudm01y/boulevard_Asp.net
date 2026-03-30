namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TempProducttableadd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TempProducts",
                c => new
                    {
                        TempId = c.Guid(nullable: false),
                        SrNo = c.String(),
                        Brand = c.String(),
                        Barcode = c.String(),
                        Category = c.String(),
                        SubCategory = c.String(),
                        ItemDesc = c.String(),
                        AttributeCode = c.String(),
                        AttributeName = c.String(),
                        Images = c.String(),
                        Quantity = c.String(),
                        SellingPrice = c.String(),
                        ProductTags = c.String(),
                        Stocks = c.String(),
                        ExcelCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TempId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TempProducts");
        }
    }
}
