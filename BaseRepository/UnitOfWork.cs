using Boulevard.Contexts;
using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.BaseRepository
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        public BoulevardDbContext _dbContext = new BoulevardDbContext();

        #region User

        private GenericRepository<User> userRepository;

        public IGenericRepository<User> UserRepository
        {
            get
            {
                if (userRepository == null)
                    this.userRepository = new GenericRepository<User>(_dbContext);
                return userRepository;
            }
        }

        #endregion

        #region Role

        private GenericRepository<Role> roleRepository;

        public IGenericRepository<Role> RoleRepository
        {
            get
            {
                if (roleRepository == null)
                    this.roleRepository = new GenericRepository<Role>(_dbContext);
                return roleRepository;
            }
        }

        #endregion

        #region Module

        private GenericRepository<Module> moduleRepository;

        public IGenericRepository<Module> ModuleRepository
        {
            get
            {
                if (moduleRepository == null)
                    this.moduleRepository = new GenericRepository<Module>(_dbContext);
                return moduleRepository;
            }
        }

        #endregion

        #region Role Module

        private GenericRepository<RoleModule> roleModuleRepository;

        public IGenericRepository<RoleModule> RoleModuleRepository
        {
            get
            {
                if (roleModuleRepository == null)
                    this.roleModuleRepository = new GenericRepository<RoleModule>(_dbContext);
                return roleModuleRepository;
            }
        }

        #endregion

        #region Layout Setting

        private GenericRepository<LayoutSetting> layoutSettingRepository;

        public IGenericRepository<LayoutSetting> LayoutSettingRepository
        {
            get
            {
                if (layoutSettingRepository == null)
                    this.layoutSettingRepository = new GenericRepository<LayoutSetting>(_dbContext);
                return layoutSettingRepository;
            }
        }

        #endregion

        #region User Activity

        private GenericRepository<UserActivity> userActivityRepository;

        public IGenericRepository<UserActivity> UserActivityRepository
        {
            get
            {
                if (userActivityRepository == null)
                    this.userActivityRepository = new GenericRepository<UserActivity>(_dbContext);
                return userActivityRepository;
            }
        }

        #endregion

        #region Member

        private GenericRepository<Models.Member> memberRepository;

        public IGenericRepository<Models.Member> MemberRepository
        {
            get
            {
                if (memberRepository == null)
                    this.memberRepository = new GenericRepository<Models.Member>(_dbContext);
                return memberRepository;
            }
        }



        #endregion
        
        #region Member Address

        private GenericRepository<Models.MemberAddress> memberAddressRepository;

        public IGenericRepository<Models.MemberAddress> MemberAddressRepository
        {
            get
            {
                if (memberAddressRepository == null)
                    this.memberAddressRepository = new GenericRepository<Models.MemberAddress>(_dbContext);
                return memberAddressRepository;
            }
        }



        #endregion


        #region Member FireBase

        private GenericRepository<MemberFirebase> memberFireBaseRepository;
        public IGenericRepository<MemberFirebase> MemberFireBaseRepository
        {
            get
            {
                if (memberFireBaseRepository == null)
                    this.memberFireBaseRepository = new GenericRepository<MemberFirebase>(_dbContext);
                return memberFireBaseRepository;
            }
        }

        #endregion


        #region Admin Notifications

        private GenericRepository<AdminNotification> adminNotificationRepository;
        public IGenericRepository<AdminNotification> AdminNotificationRepository
        {
            get
            {
                if (adminNotificationRepository == null)
                    this.adminNotificationRepository = new GenericRepository<AdminNotification>(_dbContext);
                return adminNotificationRepository;
            }
        }

        #endregion

        #region Feature Category

        private GenericRepository<FeatureCategory> featureCategoryRepository;

        public IGenericRepository<FeatureCategory> FeatureCategoryRepository
        {
            get
            {
                if (featureCategoryRepository == null)
                    this.featureCategoryRepository = new GenericRepository<FeatureCategory>(_dbContext);
                return featureCategoryRepository;
            }
        }

        #endregion


        #region Project Plan

        private GenericRepository<ProjectPlan> projectPlanRepository;

        public IGenericRepository<ProjectPlan> ProjectPlanRepository
        {
            get
            {
                if (projectPlanRepository == null)
                    this.projectPlanRepository = new GenericRepository<ProjectPlan>(_dbContext);
                return projectPlanRepository;
            }
        }

        #endregion

        #region WebHtml

        private GenericRepository<WebHtml> webHtmlRepository;
        public IGenericRepository<WebHtml> WebHtmlRepository
        {
            get
            {
                if (webHtmlRepository == null)
                    this.webHtmlRepository = new GenericRepository<WebHtml>(_dbContext);
                return webHtmlRepository;
            }
        }

        #endregion

        #region Country

        private GenericRepository<Country> countryRepository;

        public IGenericRepository<Country> CountryRepository
        {
            get
            {
                if (countryRepository == null)
                    this.countryRepository = new GenericRepository<Country>(_dbContext);
                return countryRepository;
            }
        }

        #endregion

        #region City

        private GenericRepository<City> cityRepository;

        public IGenericRepository<City> CityRepository
        {
            get
            {
                if (cityRepository == null)
                    this.cityRepository = new GenericRepository<City>(_dbContext);
                return cityRepository;
            }
        }
        #endregion

        #region Brand

        private GenericRepository<Brand> brandRepository;

        public IGenericRepository<Brand> BrandRepository
        {
            get
            {
                if (brandRepository == null)
                    this.brandRepository = new GenericRepository<Brand>(_dbContext);
                return brandRepository;
            }
        }



        #endregion

        #region Product

        private GenericRepository<Product> productRepository;

        public IGenericRepository<Product> ProductRepository
        {
            get
            {
                if (productRepository == null)
                    this.productRepository = new GenericRepository<Product>(_dbContext);
                return productRepository;
            }
        }

        #endregion

        #region ProductImage
        private GenericRepository<ProductImage> productImageRepository;

        public IGenericRepository<ProductImage> ProductImageRepository
        {
            get
            {
                if (productImageRepository == null)
                    this.productImageRepository = new GenericRepository<ProductImage>(_dbContext);
                return productImageRepository;
            }
        }
        #endregion

        #region ProductCategory

        private GenericRepository<ProductCategory> productCategoryRepository;

        public IGenericRepository<ProductCategory> ProductCategoryRepository
        {
            get
            {
                if (productCategoryRepository == null)
                    this.productCategoryRepository = new GenericRepository<ProductCategory>(_dbContext);
                return productCategoryRepository;
            }
        }
        #endregion

        #region Cart

        private GenericRepository<Cart> cartRepository;

        public IGenericRepository<Cart> CartRepository
        {
            get
            {
                if (cartRepository == null)
                    this.cartRepository = new GenericRepository<Cart>(_dbContext);
                return cartRepository;
            }
        }

        private GenericRepository<CartService> cartServiceRepository;

        public IGenericRepository<CartService> CartServiceRepository
        {
            get
            {
                if (cartServiceRepository == null)
                    this.cartServiceRepository = new GenericRepository<CartService>(_dbContext);
                return cartServiceRepository;
            }
        }


        #endregion

        #region Service

        private GenericRepository<Boulevard.Models.Service> serviceRepository;

        public IGenericRepository<Boulevard.Models.Service> ServiceRepository
        {
            get
            {
                if (serviceRepository == null)
                    this.serviceRepository = new GenericRepository<Boulevard.Models.Service>(_dbContext);
                return serviceRepository;
            }
        }
        #endregion

        #region Service Image

        private GenericRepository<ServiceImage> serviceImageRepository;

        public IGenericRepository<ServiceImage> ServiceImageRepository
        {
            get
            {
                if (serviceImageRepository == null)
                    this.serviceImageRepository = new GenericRepository<ServiceImage>(_dbContext);
                return serviceImageRepository;
            }
        }
        #endregion

        #region Service Type

        private GenericRepository<ServiceType> serviceTypesRepository;

        public IGenericRepository<ServiceType> ServiceTypesRepository
        {
            get
            {
                if (serviceTypesRepository == null)
                    this.serviceTypesRepository = new GenericRepository<ServiceType>(_dbContext);
                return serviceTypesRepository;
            }
        }
        #endregion

        #region Service Amenity

        private GenericRepository<ServiceAmenity> serviceAmenityRepository;

        public IGenericRepository<ServiceAmenity> ServiceAmenityRepository
        {
            get
            {
                if (serviceAmenityRepository == null)
                    this.serviceAmenityRepository = new GenericRepository<ServiceAmenity>(_dbContext);
                return serviceAmenityRepository;
            }
        }
        #endregion

        #region Service Category

        private GenericRepository<ServiceCategory> serviceCategoryRepository;

        public IGenericRepository<ServiceCategory> ServiceCategoryRepository
        {
            get
            {
                if (serviceCategoryRepository == null)
                    this.serviceCategoryRepository = new GenericRepository<ServiceCategory>(_dbContext);
                return serviceCategoryRepository;
            }
        }
        #endregion

        #region OrderRequest

        private GenericRepository<OrderRequestProduct> orderRequestProductRepository;

        public IGenericRepository<OrderRequestProduct> OrderRequestProductRepository
        {
            get
            {
                if (orderRequestProductRepository == null)
                    this.orderRequestProductRepository = new GenericRepository<OrderRequestProduct>(_dbContext);
                return orderRequestProductRepository;
            }
        }


        private GenericRepository<OrderRequestProductDetails> orderRequestProductDetailsRepository;

        public IGenericRepository<OrderRequestProductDetails> OrderRequestProductDetailsRepository
        {
            get
            {
                if (orderRequestProductDetailsRepository == null)
                    this.orderRequestProductDetailsRepository = new GenericRepository<OrderRequestProductDetails>(_dbContext);
                return orderRequestProductDetailsRepository;
            }
        }
        #endregion

        #region paymentmethod
        private GenericRepository<PaymentMethod> paymentMethodRepository;

        public IGenericRepository<PaymentMethod> PaymentMethodRepository
        {
            get
            {
                if (paymentMethodRepository == null)
                    this.paymentMethodRepository = new GenericRepository<PaymentMethod>(_dbContext);
                return paymentMethodRepository;
            }
        }
        #endregion

        #region Category

        private GenericRepository<Category> categoryRepository;

        public IGenericRepository<Category> CategoryRepository
        {
            get
            {
                if (categoryRepository == null)
                    this.categoryRepository = new GenericRepository<Category>(_dbContext);
                return categoryRepository;
            }
        }

        #endregion

        #region Order Status

        private GenericRepository<OrderStatus> orderStatusRepository;

        public IGenericRepository<OrderStatus> OrderStatusRepository
        {
            get
            {
                if (orderStatusRepository == null)
                    this.orderStatusRepository = new GenericRepository<OrderStatus>(_dbContext);
                return orderStatusRepository;
            }
        }

        #endregion

        #region Order Request Service

        private GenericRepository<OrderRequestService> orderRequestServiceRepository;

        public IGenericRepository<OrderRequestService> OrderRequestServiceRepository
        {
            get
            {
                if (orderRequestServiceRepository == null)
                    this.orderRequestServiceRepository = new GenericRepository<OrderRequestService>(_dbContext);
                return orderRequestServiceRepository;
            }
        }

        #endregion

        #region User Review

        private GenericRepository<UserReview> userReviewRepository;

        public IGenericRepository<UserReview> UserReviewRepository
        {
            get
            {
                if (userReviewRepository == null)
                    this.userReviewRepository = new GenericRepository<UserReview>(_dbContext);
                return userReviewRepository;
            }
        }

        #endregion

        #region User Review Image

        private GenericRepository<UserReviewImage> userReviewImageRepository;

        public IGenericRepository<UserReviewImage> UserReviewImageRepository
        {
            get
            {
                if (userReviewImageRepository == null)
                    this.userReviewImageRepository = new GenericRepository<UserReviewImage>(_dbContext);
                return userReviewImageRepository;
            }
        }

        #endregion

        #region Service Landmark

        private GenericRepository<ServiceLandmark> serviceLandmarkRepository;

        public IGenericRepository<ServiceLandmark> ServiceLandmarkRepository
        {
            get
            {
                if (serviceLandmarkRepository == null)
                    this.serviceLandmarkRepository = new GenericRepository<ServiceLandmark>(_dbContext);
                return serviceLandmarkRepository;
            }
        }

        #endregion

        #region Order Request Service

        private GenericRepository<OrderRequestServiceDetails> orderRequestServiceDetailsRepository;

        public IGenericRepository<OrderRequestServiceDetails> OrderRequestServiceDetailsRepository
        {
            get
            {
                if (orderRequestServiceDetailsRepository == null)
                    this.orderRequestServiceDetailsRepository = new GenericRepository<OrderRequestServiceDetails>(_dbContext);
                return orderRequestServiceDetailsRepository;
            }
        }

        #endregion

        #region Vehical Model

        private GenericRepository<VehicalModel> vehicalModelRepository;

        public IGenericRepository<VehicalModel> VehicalModelRepository
        {
            get
            {
                if (vehicalModelRepository == null)
                    this.vehicalModelRepository = new GenericRepository<VehicalModel>(_dbContext);
                return vehicalModelRepository;
            }
        }

        #endregion
        
        #region Vehical Model member

        private GenericRepository<MemberVehicalInfo> memberVehicalInfoRepository;

        public IGenericRepository<MemberVehicalInfo> MemberVehicalInfoRepository
        {
            get
            {
                if (memberVehicalInfoRepository == null)
                    this.memberVehicalInfoRepository = new GenericRepository<MemberVehicalInfo>(_dbContext);
                return memberVehicalInfoRepository;
            }
        }

		#endregion


		#region Faq

		private GenericRepository<FaqService> faqServiceRepository;

		public IGenericRepository<FaqService> FaqServiceRepository
		{
			get
			{
				if (faqServiceRepository == null)
					this.faqServiceRepository = new GenericRepository<FaqService>(_dbContext);
				return faqServiceRepository;
			}
		}

        #endregion

        #region Upsell

        private GenericRepository<UpsellFeatures> upsellFeaturesRepository;

        public IGenericRepository<UpsellFeatures> UpsellFeaturesRepository
        {
            get
            {
                if (upsellFeaturesRepository == null)
                    this.upsellFeaturesRepository = new GenericRepository<UpsellFeatures>(_dbContext);
                return upsellFeaturesRepository;
            }
        }

        #endregion

        #region Cross sell

        private GenericRepository<CrosssellFeatures> crosssellFeaturesRepository;

        public IGenericRepository<CrosssellFeatures> CrosssellFeaturesRepository
        {
            get
            {
                if (crosssellFeaturesRepository == null)
                    this.crosssellFeaturesRepository = new GenericRepository<CrosssellFeatures>(_dbContext);
                return crosssellFeaturesRepository;
            }
        }

        #endregion

        #region Category offers

        private GenericRepository<CategoryOffer> categoryOfferRepository;

        public IGenericRepository<CategoryOffer> CategoryOfferRepository
        {
            get
            {
                if (categoryOfferRepository == null)
                    this.categoryOfferRepository = new GenericRepository<CategoryOffer>(_dbContext);
                return categoryOfferRepository;
            }
        }

        #endregion


        #region offer information

        private GenericRepository<OfferInformation> offerInformationRepository;

        public IGenericRepository<OfferInformation> OfferInformationRepository
        {
            get
            {
                if (offerInformationRepository == null)
                    this.offerInformationRepository = new GenericRepository<OfferInformation>(_dbContext);
                return offerInformationRepository;
            }
        }

        #endregion




        #region offer Banners

        private GenericRepository<OfferBanner> offerBannerRepository;

        public IGenericRepository<OfferBanner> OfferBannerRepository
        {
            get
            {
                if (offerBannerRepository == null)
                    this.offerBannerRepository = new GenericRepository<OfferBanner>(_dbContext);
                return offerBannerRepository;
            }
        }

        #endregion


        #region offer product

        private GenericRepository<ProductOffer> productOfferRepository;

        public IGenericRepository<ProductOffer> ProductOfferRepository
        {
            get
            {
                if (productOfferRepository == null)
                    this.productOfferRepository = new GenericRepository<ProductOffer>(_dbContext);
                return productOfferRepository;
            }
        }

        #endregion


        #region offer service

        private GenericRepository<ServiceOffers> serviceOffersRepository;

        public IGenericRepository<ServiceOffers> ServiceOffersRepository
        {
            get
            {
                if (serviceOffersRepository == null)
                    this.serviceOffersRepository = new GenericRepository<ServiceOffers>(_dbContext);
                return serviceOffersRepository;
            }
        }

        #endregion


        #region offer service

        private GenericRepository<OfferDiscount> offerDiscountRepository;

        public IGenericRepository<OfferDiscount> OfferDiscountRepository
        {
            get
            {
                if (offerDiscountRepository == null)
                    this.offerDiscountRepository = new GenericRepository<OfferDiscount>(_dbContext);
                return offerDiscountRepository;
            }
        }

        #endregion

        #region offer Brand

        private GenericRepository<BrandOffer> brandOfferRepository;

        public IGenericRepository<BrandOffer> BrandOfferRepository
        {
            get
            {
                if (brandOfferRepository == null)
                    this.brandOfferRepository = new GenericRepository<BrandOffer>(_dbContext);
                return brandOfferRepository;
            }
        }

		#endregion


		#region FavouriteProduct

		private GenericRepository<FavouriteProduct> favouriteProductRepository;

		public IGenericRepository<FavouriteProduct> FavouriteProductRepository
		{
			get
			{
				if (favouriteProductRepository == null)
					this.favouriteProductRepository = new GenericRepository<FavouriteProduct>(_dbContext);
				return favouriteProductRepository;
			}
		}

		#endregion

		#region FavouriteService

		private GenericRepository<FavouriteService> favouriteServiceRepository;

		public IGenericRepository<FavouriteService> FavouriteServiceRepository
		{
			get
			{
				if (favouriteServiceRepository == null)
					this.favouriteServiceRepository = new GenericRepository<FavouriteService>(_dbContext);
				return favouriteServiceRepository;
			}
		}

        #endregion


        #region DeliverySetting

        private GenericRepository<DeliverySetting> deliverySettingRepository;

        public IGenericRepository<DeliverySetting> DeliverySettingRepository
        {
            get
            {
                if (deliverySettingRepository == null)
                    this.deliverySettingRepository = new GenericRepository<DeliverySetting>(_dbContext);
                return deliverySettingRepository;
            }
        }

        #endregion

        #region User Report

        private GenericRepository<UserReport> userReportRepository;

        public IGenericRepository<UserReport> UserReportRepository
        {
            get
            {
                if (userReportRepository == null)
                    this.userReportRepository = new GenericRepository<UserReport>(_dbContext);
                return userReportRepository;
            }
        }

        #endregion

        #region User Report Details

        private GenericRepository<UserReportDetails> userReportDetailsRepository;

        public IGenericRepository<UserReportDetails> UserReportDetailsRepository
        {
            get
            {
                if (userReportDetailsRepository == null)
                    this.userReportDetailsRepository = new GenericRepository<UserReportDetails>(_dbContext);
                return userReportDetailsRepository;
            }
        }

        #endregion


        #region Customer Enquery

        private GenericRepository<CustomerEnquery> customerEnqueryRepository;

        public IGenericRepository<CustomerEnquery> CustomerEnqueryRepository
        {
            get
            {
                if (customerEnqueryRepository == null)
                    this.customerEnqueryRepository = new GenericRepository<CustomerEnquery>(_dbContext);
                return customerEnqueryRepository;
            }
        }

        #endregion


        #region Common Product

        private GenericRepository<CommonProductTag> commonProductTagRepository;

        public IGenericRepository<CommonProductTag> CommonProductTagRepository
        {
            get
            {
                if (commonProductTagRepository == null)
                    this.commonProductTagRepository = new GenericRepository<CommonProductTag>(_dbContext);
                return commonProductTagRepository;
            }
        }

        #endregion


        #region Common Product Details

        private GenericRepository<CommonProductTagDetails> commonProductTagDetailsRepository;

        public IGenericRepository<CommonProductTagDetails> CommonProductTagDetailsRepository
        {
            get
            {
                if (commonProductTagDetailsRepository == null)
                    this.commonProductTagDetailsRepository = new GenericRepository<CommonProductTagDetails>(_dbContext);
                return commonProductTagDetailsRepository;
            }
        }

        #endregion


        #region membership

        private GenericRepository<MemberShip> memberShipRepository;

        public IGenericRepository<MemberShip> MemberShipRepository
        {
            get
            {
                if (memberShipRepository == null)
                    this.memberShipRepository = new GenericRepository<MemberShip>(_dbContext);
                return memberShipRepository;
            }
        }



        private GenericRepository<MemberShipDiscountCategory> memberShipDiscountCategoryRepository;

        public IGenericRepository<MemberShipDiscountCategory> MemberShipDiscountCategoryRepository
        {
            get
            {
                if (memberShipDiscountCategoryRepository == null)
                    this.memberShipDiscountCategoryRepository = new GenericRepository<MemberShipDiscountCategory>(_dbContext);
                return memberShipDiscountCategoryRepository;
            }
        }


        private GenericRepository<MemberSubscription> memberSubscriptionRepository;

        public IGenericRepository<MemberSubscription> MemberSubscriptionRepository
        {
            get
            {
                if (memberSubscriptionRepository == null)
                    this.memberSubscriptionRepository = new GenericRepository<MemberSubscription>(_dbContext);
                return memberSubscriptionRepository;
            }
        }

        private GenericRepository<AirportService> airportServiceRepository;

        public IGenericRepository<AirportService> AirportServiceRepository
        {
            get
            {
                if (airportServiceRepository == null)
                    this.airportServiceRepository = new GenericRepository<AirportService>(_dbContext);
                return airportServiceRepository;
            }
        }

        private GenericRepository<Airport> airportRepository;

        public IGenericRepository<Airport> AirportRepository
        {
            get
            {
                if (airportRepository == null)
                    this.airportRepository = new GenericRepository<Airport>(_dbContext);
                return airportRepository;
            }
        }

        #endregion

        #region StockLog
        private GenericRepository<StockLog> stockLogRepository;

        public IGenericRepository<StockLog> StockLogRepository
        {
            get
            {
                if (stockLogRepository == null)
                    this.stockLogRepository = new GenericRepository<StockLog>(_dbContext);
                return stockLogRepository;
            }
        }
        #endregion


        #region PropertyInformation
        private GenericRepository<PropertyInformation> propertyInformationRepository;

        public IGenericRepository<PropertyInformation> PropertyInformationRepository
        {
            get
            {
                if (propertyInformationRepository == null)
                    this.propertyInformationRepository = new GenericRepository<PropertyInformation>(_dbContext);
                return propertyInformationRepository;
            }
        }
        #endregion



        #region Service Type Amnity
        private GenericRepository<ServiceTypeAmenity> serviceTypeAmenityRepository;

        public IGenericRepository<ServiceTypeAmenity> ServiceTypeAmenityRepository
        {
            get
            {
                if (serviceTypeAmenityRepository == null)
                    this.serviceTypeAmenityRepository = new GenericRepository<ServiceTypeAmenity>(_dbContext);
                return serviceTypeAmenityRepository;
            }
        }
        #endregion



        #region Service Type File
        private GenericRepository<ServiceTypeFile> serviceTypeFileRepository;

        public IGenericRepository<ServiceTypeFile> ServiceTypeFileRepository
        {
            get
            {
                if (serviceTypeFileRepository == null)
                    this.serviceTypeFileRepository = new GenericRepository<ServiceTypeFile>(_dbContext);
                return serviceTypeFileRepository;
            }
        }
        #endregion

        #region Product Price
        private GenericRepository<ProductPrice> _productPriceRepository;

        public IGenericRepository<ProductPrice> ProductPriceRepository
        {
            get
            {
                if (_productPriceRepository == null)
                    this._productPriceRepository = new GenericRepository<ProductPrice>(_dbContext);
                return _productPriceRepository;
            }
        }
        #endregion

        #region notification
        private GenericRepository<Notification> _notificationRepository;

        public IGenericRepository<Notification> NotificationRepository
        {
            get
            {
                if (_notificationRepository == null)
                    this._notificationRepository = new GenericRepository<Notification>(_dbContext);
                return _notificationRepository;
            }
        }
        #endregion


        #region notification
        private GenericRepository<ProductTypeMaster> _productTypeRepository;

        public IGenericRepository<ProductTypeMaster> ProductTypeMasterRepository
        {
            get
            {
                if (_productTypeRepository == null)
                    this._productTypeRepository = new GenericRepository<ProductTypeMaster>(_dbContext);
                return _productTypeRepository;
            }
        }
        #endregion

        #region notification
        private GenericRepository<OrderMasterStatusLog> _orderMasterStatusLogRepository;

        public IGenericRepository<OrderMasterStatusLog> OrderMasterStatusLogRepository
        {
            get
            {
                if (_orderMasterStatusLogRepository == null)
                    this._orderMasterStatusLogRepository = new GenericRepository<OrderMasterStatusLog>(_dbContext);
                return _orderMasterStatusLogRepository;
            }
        }
        #endregion

        #region ServiceFormConfiguration
        private GenericRepository<ServiceFormSection> _serviceFormSectionRepository;
        public IGenericRepository<ServiceFormSection> ServiceFormSectionRepository
        {
            get
            {
                if (_serviceFormSectionRepository == null)
                    this._serviceFormSectionRepository = new GenericRepository<ServiceFormSection>(_dbContext);
                return _serviceFormSectionRepository;
            }
        }

        private GenericRepository<ServiceFormField> _serviceFormFieldRepository;
        public IGenericRepository<ServiceFormField> ServiceFormFieldRepository
        {
            get
            {
                if (_serviceFormFieldRepository == null)
                    this._serviceFormFieldRepository = new GenericRepository<ServiceFormField>(_dbContext);
                return _serviceFormFieldRepository;
            }
        }

        private GenericRepository<ServiceFormFieldOption> _serviceFormFieldOptionRepository;
        public IGenericRepository<ServiceFormFieldOption> ServiceFormFieldOptionRepository
        {
            get
            {
                if (_serviceFormFieldOptionRepository == null)
                    this._serviceFormFieldOptionRepository = new GenericRepository<ServiceFormFieldOption>(_dbContext);
                return _serviceFormFieldOptionRepository;
            }
        }

        private GenericRepository<ServiceFormAttachmentRule> _serviceFormAttachmentRuleRepository;
        public IGenericRepository<ServiceFormAttachmentRule> ServiceFormAttachmentRuleRepository
        {
            get
            {
                if (_serviceFormAttachmentRuleRepository == null)
                    this._serviceFormAttachmentRuleRepository = new GenericRepository<ServiceFormAttachmentRule>(_dbContext);
                return _serviceFormAttachmentRuleRepository;
            }
        }
        #endregion

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            this.disposed = true; 
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}