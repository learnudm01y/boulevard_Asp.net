namespace Boulevard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Languagesetupforalldatabase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cities", "CityNameAr", c => c.String());
            AddColumn("dbo.Countries", "CountryNameAr", c => c.String());
            AddColumn("dbo.Services", "NameAr", c => c.String(maxLength: 100));
            AddColumn("dbo.Services", "DescriptionAr", c => c.String());
            AddColumn("dbo.Services", "AboutUsAr", c => c.String());
            AddColumn("dbo.Services", "ScopeOfServiceAr", c => c.String());
            AddColumn("dbo.ServiceAmenities", "AmenitiesNameAr", c => c.String(maxLength: 100));
            AddColumn("dbo.ServiceTypes", "ServiceTypeNameAr", c => c.String(maxLength: 250));
            AddColumn("dbo.ServiceTypes", "DescriptionAr", c => c.String());
            AddColumn("dbo.ServiceTypes", "SizeAr", c => c.String(maxLength: 100));
            AddColumn("dbo.Brands", "TitleAr", c => c.String(maxLength: 250));
            AddColumn("dbo.Brands", "DetailsAr", c => c.String());
            AddColumn("dbo.OfferInformations", "TitleAr", c => c.String(maxLength: 250));
            AddColumn("dbo.OfferInformations", "DescriptionAr", c => c.String());
            AddColumn("dbo.OfferInformations", "FeatureTypeAr", c => c.String(maxLength: 100));
            AddColumn("dbo.Categories", "CategoryNameAr", c => c.String(maxLength: 250));
            AddColumn("dbo.Categories", "CategoryDescriptionAr", c => c.String());
            AddColumn("dbo.CommonProductTags", "TagNameAr", c => c.String(maxLength: 250));
            AddColumn("dbo.FaqServices", "FaqTitleAr", c => c.String());
            AddColumn("dbo.FaqServices", "FaqDescriptionAr", c => c.String());
            AddColumn("dbo.FaqServices", "FeatureTypeAr", c => c.String());
            AddColumn("dbo.Products", "ProductNameAr", c => c.String(maxLength: 250));
            AddColumn("dbo.Products", "ProductSlagAr", c => c.String(maxLength: 250));
            AddColumn("dbo.Products", "ProductDescriptionAr", c => c.String());
            AddColumn("dbo.MemberShipDiscountCategories", "MemberShipDiscountTypeAr", c => c.String(maxLength: 100));
            AddColumn("dbo.MemberShips", "TitleAr", c => c.String(maxLength: 250));
            AddColumn("dbo.MemberShips", "DescriptionAr", c => c.String(maxLength: 500));
            AddColumn("dbo.MemberShips", "BenefitsAr", c => c.String());
            AddColumn("dbo.MemberShips", "MembershipBannerAr", c => c.String(maxLength: 250));
            AddColumn("dbo.Modules", "ModuleNameAr", c => c.String());
            AddColumn("dbo.OfferDiscounts", "DiscountTypeAr", c => c.String(maxLength: 100));
            AddColumn("dbo.PaymentMethods", "PaymentMethodNameAr", c => c.String());
            AddColumn("dbo.PropertyInformations", "TypeAr", c => c.String(maxLength: 100));
            AddColumn("dbo.PropertyInformations", "PurposeAr", c => c.String(maxLength: 100));
            AddColumn("dbo.PropertyInformations", "RefNoAr", c => c.String(maxLength: 100));
            AddColumn("dbo.PropertyInformations", "FurnishingAr", c => c.String(maxLength: 100));
            AddColumn("dbo.PropertyInformations", "ExteriorsAr", c => c.String());
            AddColumn("dbo.PropertyInformations", "InteriorsAr", c => c.String());
            AddColumn("dbo.ServiceTypeAmenities", "AmenitiesNameAr", c => c.String(maxLength: 100));
            AddColumn("dbo.UpsellFeatures", "UpsellFeaturesTypeAr", c => c.String(maxLength: 50));
            AddColumn("dbo.WebHtmls", "TitleAr", c => c.String(maxLength: 250));
            AddColumn("dbo.WebHtmls", "SubTitleAr", c => c.String(maxLength: 500));
            AddColumn("dbo.WebHtmls", "SmallDetailsOneAr", c => c.String(maxLength: 500));
            AddColumn("dbo.WebHtmls", "SmallDetailsTwoAr", c => c.String(maxLength: 500));
            AddColumn("dbo.WebHtmls", "BigDetailsOneAr", c => c.String());
            AddColumn("dbo.WebHtmls", "BigDetailsTwoAr", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.WebHtmls", "BigDetailsTwoAr");
            DropColumn("dbo.WebHtmls", "BigDetailsOneAr");
            DropColumn("dbo.WebHtmls", "SmallDetailsTwoAr");
            DropColumn("dbo.WebHtmls", "SmallDetailsOneAr");
            DropColumn("dbo.WebHtmls", "SubTitleAr");
            DropColumn("dbo.WebHtmls", "TitleAr");
            DropColumn("dbo.UpsellFeatures", "UpsellFeaturesTypeAr");
            DropColumn("dbo.ServiceTypeAmenities", "AmenitiesNameAr");
            DropColumn("dbo.PropertyInformations", "InteriorsAr");
            DropColumn("dbo.PropertyInformations", "ExteriorsAr");
            DropColumn("dbo.PropertyInformations", "FurnishingAr");
            DropColumn("dbo.PropertyInformations", "RefNoAr");
            DropColumn("dbo.PropertyInformations", "PurposeAr");
            DropColumn("dbo.PropertyInformations", "TypeAr");
            DropColumn("dbo.PaymentMethods", "PaymentMethodNameAr");
            DropColumn("dbo.OfferDiscounts", "DiscountTypeAr");
            DropColumn("dbo.Modules", "ModuleNameAr");
            DropColumn("dbo.MemberShips", "MembershipBannerAr");
            DropColumn("dbo.MemberShips", "BenefitsAr");
            DropColumn("dbo.MemberShips", "DescriptionAr");
            DropColumn("dbo.MemberShips", "TitleAr");
            DropColumn("dbo.MemberShipDiscountCategories", "MemberShipDiscountTypeAr");
            DropColumn("dbo.Products", "ProductDescriptionAr");
            DropColumn("dbo.Products", "ProductSlagAr");
            DropColumn("dbo.Products", "ProductNameAr");
            DropColumn("dbo.FaqServices", "FeatureTypeAr");
            DropColumn("dbo.FaqServices", "FaqDescriptionAr");
            DropColumn("dbo.FaqServices", "FaqTitleAr");
            DropColumn("dbo.CommonProductTags", "TagNameAr");
            DropColumn("dbo.Categories", "CategoryDescriptionAr");
            DropColumn("dbo.Categories", "CategoryNameAr");
            DropColumn("dbo.OfferInformations", "FeatureTypeAr");
            DropColumn("dbo.OfferInformations", "DescriptionAr");
            DropColumn("dbo.OfferInformations", "TitleAr");
            DropColumn("dbo.Brands", "DetailsAr");
            DropColumn("dbo.Brands", "TitleAr");
            DropColumn("dbo.ServiceTypes", "SizeAr");
            DropColumn("dbo.ServiceTypes", "DescriptionAr");
            DropColumn("dbo.ServiceTypes", "ServiceTypeNameAr");
            DropColumn("dbo.ServiceAmenities", "AmenitiesNameAr");
            DropColumn("dbo.Services", "ScopeOfServiceAr");
            DropColumn("dbo.Services", "AboutUsAr");
            DropColumn("dbo.Services", "DescriptionAr");
            DropColumn("dbo.Services", "NameAr");
            DropColumn("dbo.Countries", "CountryNameAr");
            DropColumn("dbo.Cities", "CityNameAr");
        }
    }
}
