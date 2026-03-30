namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateservicetempforrealstate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TempServices", "ServiceTypeCategory", c => c.String());
            AddColumn("dbo.TempServices", "ServiceTypeSubCategory", c => c.String());
            AddColumn("dbo.TempServices", "PropertyType", c => c.String());
            AddColumn("dbo.TempServices", "PropertyTypeArabic", c => c.String());
            AddColumn("dbo.TempServices", "PropertyRefNo", c => c.String());
            AddColumn("dbo.TempServices", "PropertyPurpose", c => c.String());
            AddColumn("dbo.TempServices", "Furnishing", c => c.String());
            AddColumn("dbo.TempServices", "FurnishingArabic", c => c.String());
            AddColumn("dbo.TempServices", "PropertyWhatsAppNo", c => c.String());
            AddColumn("dbo.TempServices", "PropertyEmail", c => c.String());
            AddColumn("dbo.TempServices", "ExteriorDetails", c => c.String());
            AddColumn("dbo.TempServices", "ExteriorDetailsArabic", c => c.String());
            AddColumn("dbo.TempServices", "ExteriorImage", c => c.String());
            AddColumn("dbo.TempServices", "InteriorDetails", c => c.String());
            AddColumn("dbo.TempServices", "InteriorDetailsArabic", c => c.String());
            AddColumn("dbo.TempServices", "InteriorImage", c => c.String());
            AddColumn("dbo.TempServices", "AmenitiesFile", c => c.String());
            AddColumn("dbo.TempServices", "CloserPropertyName", c => c.String());
            AddColumn("dbo.TempServices", "CloserPropertyNameArabic", c => c.String());
            AddColumn("dbo.TempServices", "CloserPropertyLogo", c => c.String());
            AddColumn("dbo.TempServices", "CloserPropertyFile", c => c.String());
            AddColumn("dbo.TempServices", "MaterialsName", c => c.String());
            AddColumn("dbo.TempServices", "MaterialsNameArabic", c => c.String());
            AddColumn("dbo.TempServices", "MaterialsLogo", c => c.String());
            AddColumn("dbo.TempServices", "MaterialsFile", c => c.String());
            AddColumn("dbo.TempServices", "UtilityName", c => c.String());
            AddColumn("dbo.TempServices", "UtilityNameArabic", c => c.String());
            AddColumn("dbo.TempServices", "UtilityLogo", c => c.String());
            AddColumn("dbo.TempServices", "UtilityFile", c => c.String());
            AddColumn("dbo.TempServices", "Video", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TempServices", "Video");
            DropColumn("dbo.TempServices", "UtilityFile");
            DropColumn("dbo.TempServices", "UtilityLogo");
            DropColumn("dbo.TempServices", "UtilityNameArabic");
            DropColumn("dbo.TempServices", "UtilityName");
            DropColumn("dbo.TempServices", "MaterialsFile");
            DropColumn("dbo.TempServices", "MaterialsLogo");
            DropColumn("dbo.TempServices", "MaterialsNameArabic");
            DropColumn("dbo.TempServices", "MaterialsName");
            DropColumn("dbo.TempServices", "CloserPropertyFile");
            DropColumn("dbo.TempServices", "CloserPropertyLogo");
            DropColumn("dbo.TempServices", "CloserPropertyNameArabic");
            DropColumn("dbo.TempServices", "CloserPropertyName");
            DropColumn("dbo.TempServices", "AmenitiesFile");
            DropColumn("dbo.TempServices", "InteriorImage");
            DropColumn("dbo.TempServices", "InteriorDetailsArabic");
            DropColumn("dbo.TempServices", "InteriorDetails");
            DropColumn("dbo.TempServices", "ExteriorImage");
            DropColumn("dbo.TempServices", "ExteriorDetailsArabic");
            DropColumn("dbo.TempServices", "ExteriorDetails");
            DropColumn("dbo.TempServices", "PropertyEmail");
            DropColumn("dbo.TempServices", "PropertyWhatsAppNo");
            DropColumn("dbo.TempServices", "FurnishingArabic");
            DropColumn("dbo.TempServices", "Furnishing");
            DropColumn("dbo.TempServices", "PropertyPurpose");
            DropColumn("dbo.TempServices", "PropertyRefNo");
            DropColumn("dbo.TempServices", "PropertyTypeArabic");
            DropColumn("dbo.TempServices", "PropertyType");
            DropColumn("dbo.TempServices", "ServiceTypeSubCategory");
            DropColumn("dbo.TempServices", "ServiceTypeCategory");
        }
    }
}
