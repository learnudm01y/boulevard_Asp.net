using Boulevard.Models;
using Boulevard.Service;
using Boulevard.Service.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boulevard.BaseRepository
{
    public interface IUnitOfWork
    {
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<Role> RoleRepository { get; }
        IGenericRepository<Module> ModuleRepository { get; }
        IGenericRepository<RoleModule> RoleModuleRepository { get; }
        IGenericRepository<LayoutSetting> LayoutSettingRepository { get; }
        IGenericRepository<UserActivity> UserActivityRepository { get; }
        IGenericRepository<Member> MemberRepository { get; }
        IGenericRepository<MemberAddress> MemberAddressRepository { get; }

        IGenericRepository<MemberFirebase> MemberFireBaseRepository { get; }

        IGenericRepository<FeatureCategory> FeatureCategoryRepository { get; }

        IGenericRepository<WebHtml> WebHtmlRepository { get; }
        IGenericRepository<Country> CountryRepository { get; }
        IGenericRepository<City> CityRepository { get; }

        IGenericRepository<Brand> BrandRepository { get; }

        IGenericRepository<Category> CategoryRepository { get; }

        IGenericRepository<Product> ProductRepository { get; }

        IGenericRepository<ProductImage> ProductImageRepository { get; }

        IGenericRepository<ProductCategory> ProductCategoryRepository { get; }

        IGenericRepository<Cart> CartRepository { get; }
        IGenericRepository<Boulevard.Models.Service> ServiceRepository { get; }

        IGenericRepository<OrderRequestProduct> OrderRequestProductRepository { get; }

        IGenericRepository<OrderRequestProductDetails> OrderRequestProductDetailsRepository { get; }

        IGenericRepository<PaymentMethod> PaymentMethodRepository { get; }
  
        IGenericRepository<ServiceImage> ServiceImageRepository { get; }
        IGenericRepository<ServiceAmenity> ServiceAmenityRepository { get; }
        IGenericRepository<ServiceType> ServiceTypesRepository { get; }
        IGenericRepository<OrderRequestService> OrderRequestServiceRepository { get; }
        IGenericRepository<UserReview> UserReviewRepository { get; }
        IGenericRepository<UserReviewImage> UserReviewImageRepository { get; }
        IGenericRepository<ServiceLandmark> ServiceLandmarkRepository { get; }

        IGenericRepository<OrderStatus> OrderStatusRepository { get; }
        IGenericRepository<OrderRequestServiceDetails> OrderRequestServiceDetailsRepository { get; }
        IGenericRepository<VehicalModel> VehicalModelRepository { get; }

        IGenericRepository<ServiceCategory> ServiceCategoryRepository { get; }
        IGenericRepository<MemberVehicalInfo> MemberVehicalInfoRepository { get; }

		IGenericRepository<FaqService> FaqServiceRepository { get; }

        IGenericRepository<UpsellFeatures> UpsellFeaturesRepository { get; }

        IGenericRepository<CrosssellFeatures> CrosssellFeaturesRepository { get; }


        IGenericRepository<CategoryOffer> CategoryOfferRepository { get; }


        IGenericRepository<OfferBanner> OfferBannerRepository { get; }


        IGenericRepository<OfferInformation> OfferInformationRepository { get; }


        IGenericRepository<ProductOffer> ProductOfferRepository { get; }


        IGenericRepository<ServiceOffers> ServiceOffersRepository { get; }

        IGenericRepository<OfferDiscount> OfferDiscountRepository { get; }

        IGenericRepository<BrandOffer> BrandOfferRepository { get; }

		IGenericRepository<FavouriteService> FavouriteServiceRepository { get; }
		IGenericRepository<FavouriteProduct> FavouriteProductRepository { get; }

        IGenericRepository<DeliverySetting> DeliverySettingRepository { get; }
        IGenericRepository<UserReport> UserReportRepository { get; }
        IGenericRepository<UserReportDetails> UserReportDetailsRepository { get; }

        IGenericRepository<CustomerEnquery> CustomerEnqueryRepository { get; }
        IGenericRepository<CommonProductTag> CommonProductTagRepository { get; }

        IGenericRepository<CommonProductTagDetails> CommonProductTagDetailsRepository { get; }

        IGenericRepository<MemberShip> MemberShipRepository { get; }

        IGenericRepository<MemberShipDiscountCategory> MemberShipDiscountCategoryRepository { get; }

        IGenericRepository<MemberSubscription> MemberSubscriptionRepository { get; }
        IGenericRepository<Airport> AirportRepository { get; }
        IGenericRepository<AirportService> AirportServiceRepository { get; }
        IGenericRepository<StockLog> StockLogRepository { get; }


        IGenericRepository<PropertyInformation> PropertyInformationRepository { get; }

        IGenericRepository<ServiceTypeAmenity> ServiceTypeAmenityRepository { get; }

        IGenericRepository<ServiceTypeFile> ServiceTypeFileRepository { get; }
        IGenericRepository<ProductPrice> ProductPriceRepository { get; }

        IGenericRepository<Notification> NotificationRepository { get; }

        IGenericRepository<ProductTypeMaster> ProductTypeMasterRepository { get; }

        IGenericRepository<OrderMasterStatusLog> OrderMasterStatusLogRepository { get; }

        IGenericRepository<CartService> CartServiceRepository { get; }

        IGenericRepository<AdminNotification> AdminNotificationRepository { get; }

        IGenericRepository<ProjectPlan> ProjectPlanRepository { get; }

        // Service Form Configuration
        IGenericRepository<ServiceFormSection> ServiceFormSectionRepository { get; }
        IGenericRepository<ServiceFormField> ServiceFormFieldRepository { get; }
        IGenericRepository<ServiceFormFieldOption> ServiceFormFieldOptionRepository { get; }
        IGenericRepository<ServiceFormAttachmentRule> ServiceFormAttachmentRuleRepository { get; }


    }
}
