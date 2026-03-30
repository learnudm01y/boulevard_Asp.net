namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewtables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PropertyInformations",
                c => new
                    {
                        PropertyInformationId = c.Int(nullable: false, identity: true),
                        PropertyInfoKey = c.Guid(nullable: false),
                        Type = c.String(maxLength: 100),
                        Purpose = c.String(maxLength: 100),
                        RefNo = c.String(maxLength: 100),
                        Furnishing = c.String(maxLength: 100),
                        PropertyWhatsAppNo = c.String(maxLength: 100),
                        PropertyPhoneNo = c.String(maxLength: 100),
                        PropertyEmail = c.String(maxLength: 100),
                        ServiceTypeId = c.Int(),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.PropertyInformationId)
                .ForeignKey("dbo.ServiceTypes", t => t.ServiceTypeId)
                .Index(t => t.ServiceTypeId);
            
            CreateTable(
                "dbo.ServiceTypeAmenities",
                c => new
                    {
                        ServiceAmenityId = c.Int(nullable: false, identity: true),
                        ServiceAmenityKey = c.Guid(nullable: false),
                        ServiceTypeId = c.Int(),
                        AmenitiesName = c.String(maxLength: 100),
                        AmenitiesLogo = c.String(maxLength: 100),
                        AmenitiesType = c.String(maxLength: 100),
                        LinkedWithFile = c.Boolean(nullable: false),
                        Status = c.String(maxLength: 10),
                        CreateBy = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateBy = c.Int(),
                        UpdateDate = c.DateTime(),
                        DeleteBy = c.Int(),
                        DeleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ServiceAmenityId)
                .ForeignKey("dbo.ServiceTypes", t => t.ServiceTypeId)
                .Index(t => t.ServiceTypeId);
            
            CreateTable(
                "dbo.ServiceTypeFiles",
                c => new
                    {
                        ServiceTypeFileId = c.Int(nullable: false, identity: true),
                        ServiceTypeId = c.Int(),
                        FileSource = c.String(maxLength: 180),
                        FileType = c.String(maxLength: 180),
                        FileLocation = c.String(maxLength: 180),
                        LastUpdate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ServiceTypeFileId)
                .ForeignKey("dbo.ServiceTypes", t => t.ServiceTypeId)
                .Index(t => t.ServiceTypeId);
            
            AddColumn("dbo.ServiceTypes", "PaymentType", c => c.String(maxLength: 100));
            AddColumn("dbo.ServiceTypes", "Latitute", c => c.String(maxLength: 100));
            AddColumn("dbo.ServiceTypes", "Logitute", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceTypeFiles", "ServiceTypeId", "dbo.ServiceTypes");
            DropForeignKey("dbo.ServiceTypeAmenities", "ServiceTypeId", "dbo.ServiceTypes");
            DropForeignKey("dbo.PropertyInformations", "ServiceTypeId", "dbo.ServiceTypes");
            DropIndex("dbo.ServiceTypeFiles", new[] { "ServiceTypeId" });
            DropIndex("dbo.ServiceTypeAmenities", new[] { "ServiceTypeId" });
            DropIndex("dbo.PropertyInformations", new[] { "ServiceTypeId" });
            DropColumn("dbo.ServiceTypes", "Logitute");
            DropColumn("dbo.ServiceTypes", "Latitute");
            DropColumn("dbo.ServiceTypes", "PaymentType");
            DropTable("dbo.ServiceTypeFiles");
            DropTable("dbo.ServiceTypeAmenities");
            DropTable("dbo.PropertyInformations");
        }
    }
}
