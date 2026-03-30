using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Boulevard.Contexts
{
    public class BoulevardDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<RoleModule> RoleModules { get; set; }
        public DbSet<LayoutSetting> LayoutSettings { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }

        public DbSet<WebHtml> webHtmls { get; set; }
        public DbSet<FeatureCategory> featureCategories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        public DbSet<Member> Members { get; set; }
        public DbSet<MemberAddress> MemberAddresses { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }

        public DbSet<MemberFirebase> MemberFirebases { get; set; }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Models.Service> Services { get; set; }

        public DbSet<ServiceAmenity> ServiceAmenities { get; set; }

        public DbSet<ServiceImage> ServiceImages { get; set; }

        public DbSet<ServiceType> ServiceTypes { get; set; }
        public DbSet<UserReview> UserReviewes { get; set; }
        public DbSet<UserReviewImage> UserReviewImages { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<OrderRequestProduct> OrderRequestProductss { get; set; }
        public DbSet<OrderRequestProductDetails> OrderRequestProductDetails { get; set; }

        public DbSet<PaymentMethod> PaymentMethod { get; set; }

        public DbSet<OrderStatus> OrderStatus { get; set; }

        public DbSet<OrderRequestService> OrderRequestService { get; set; }

        public DbSet<ServiceLandmark> ServiceLandmark { get; set; }

        public DbSet<OrderRequestServiceDetails> OrderRequestServiceDetails { get; set; }

        public DbSet<FaqService> FaqServices { get; set; }

        public DbSet<VehicalModel> VehicalModels { get; set; }

        public DbSet<MemberVehicalInfo> MemberVehicalInfo { get; set; }

        public DbSet<ServiceCategory> ServiceCategories { get; set; }

        public DbSet<UpsellFeatures> UpsellFeatures { get; set; }
        public DbSet<CrosssellFeatures> CrosssellFeatures { get; set; }
        public DbSet<OfferInformation> OfferInformations { get; set; }
        public DbSet<OfferBanner> OfferBanner { get; set; }
        public DbSet<BrandOffer> BrandOffers { get; set; }
        public DbSet<CategoryOffer> CategoryOffers { get; set; }

        public DbSet<ProductOffer> ProductOffers { get; set; }

        public DbSet<ServiceOffers> ServiceOffers { get; set; }

        public DbSet<OfferDiscount> OfferDiscounts { get; set; }

		public DbSet<FavouriteProduct> FavouriteProducts { get; set; }

		public DbSet<FavouriteService> FavouriteServices { get; set; }

        public DbSet<DeliverySetting> DeliverySettings { get; set; }
        public DbSet<UserReport> UserReports { get; set; }
        public DbSet<UserReportDetails> UserReportDetails { get; set; }

        public DbSet<CustomerEnquery> CustomerEnquery { get; set; }

        public DbSet<CommonProductTag> CommonProductTags { get; set; }

        public DbSet<CommonProductTagDetails> CommonProductTagDetails { get; set; }

        public DbSet<MemberShip> MemberShips { get; set; }

        public DbSet<MemberShipDiscountCategory> MemberShipDiscountCategories { get; set; }

        public DbSet<MemberSubscription> MemberSubscriptions { get; set; }
        public DbSet<Airport> Airports { get; set; }
        public DbSet<AirportService> AirportServices { get; set; } 
        public DbSet<StockLog> StockLogs { get; set; }


        public DbSet<PropertyInformation> PropertyInformations { get; set; }

        public DbSet<ServiceTypeAmenity> ServiceTypeAmenities { get; set; }

        public DbSet<ServiceTypeFile> ServiceTypeFiles { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<TempProduct> TempProducts { get; set; }
        public DbSet<TempService> TempServices { get; set; }
        public DbSet<TempServiceType> TempServiceTypes { get; set; }
        public DbSet<MonthlyGoal> MonthlyGoals { get; set; }
        public DbSet<GolbalMemberCategory> GolbalMemberCategories { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<ProductTypeMaster> ProductTypeMaster { get; set; }

        public DbSet<OrderMasterStatusLog> OrderMasterStatusLog { get; set; }

        public DbSet<CartService> CartService { get; set; }

        public DbSet<AdminNotification> AdminNotifications { get; set; }

        public DbSet<ProjectPlan> ProjectPlan { get; set; }

    }
}