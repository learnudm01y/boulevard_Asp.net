using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;


namespace Boulevard.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            // Web API configuration and services
            var settings = GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();


            #region FeatureCategory

            config.Routes.MapHttpRoute(
               "GetAllfeatureCategory",
               "api/v1/GetAllfeatureCategory",
               new { controller = "FeatureCategory", action = "GetAllFeatureCategory", id = RouteParameter.Optional }
            );
            #endregion

            #region Member

            config.Routes.MapHttpRoute(
               "member_Register",
               "api/v1/member/register",
               new { controller = "Member", action = "Register", id = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
            "post_member_login",
            "api/v1/member/login",
            new { controller = "Member", action = "Login", id = RouteParameter.Optional }
          );

            config.Routes.MapHttpRoute(
       "post_member_LoginWithEmail",
       "api/v1/member/LoginWithEmail",
       new { controller = "Member", action = "LoginWithEmail", id = RouteParameter.Optional }
     );

            config.Routes.MapHttpRoute(
     "post_member_RegistrationFromThirdParty",
     "api/v1/member/RegistrationFromThirdParty",
     new { controller = "Member", action = "RegistrationFromThirdParty", id = RouteParameter.Optional }
   );
            
            config.Routes.MapHttpRoute(
          "post_member_OTPCheck",
          "api/v1/member/otp-check/{Otp}/{memberId}",
          new { controller = "Member", action = "OTPCheck", Otp = RouteParameter.Optional, memberId = RouteParameter.Optional }
        );


            config.Routes.MapHttpRoute(
              "member_Details",
              "api/v1/member/memberDetails/{id}",
              new { controller = "Member", action = "MemberDetails", id = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
         "member_DetailsV2",
         "api/v1/member/memberDetailsV2/{id}",
         new { controller = "Member", action = "MemberDetailsV2", id = RouteParameter.Optional }
      );



            config.Routes.MapHttpRoute(
             "member_Edit",
             "api/v1/member/memberEdit",
             new { controller = "Member", action = "ProfileEdit", id = RouteParameter.Optional }
          );


            config.Routes.MapHttpRoute(
       "UpdateMemberPassword",
       "api/v1/member/UpdatePassword",
       new { controller = "Member", action = "UpdateMemberPassword", id = RouteParameter.Optional }
    );


            config.Routes.MapHttpRoute(
"MemberActive",
"api/v1/member/MemberActive",
new { controller = "Member", action = "MemberActive", id = RouteParameter.Optional }
);

            config.Routes.MapHttpRoute(
      "Member_password_forgetV2",
      "api/v1/member/password-forgetV2",
      new { controller = "Member", action = "MemberForgetPasswordV2", id = RouteParameter.Optional }
  );

            config.Routes.MapHttpRoute(
      "Member_MemberDelete",
      "api/v1/member/MemberDelete",
      new { controller = "Member", action = "MemberDelete", id = RouteParameter.Optional }
  );
            

            #endregion


            #region AddPost

            config.Routes.MapHttpRoute(
           "AddPostImage",
           "api/v1/Post/AddPostImages",
           new { controller = "Upload", action = "PostImages", id = RouteParameter.Optional }
       );








            config.Routes.MapHttpRoute(
           "AddPostFiles",
           "api/v1/Post/AddPostFiles",
           new { controller = "Upload", action = "PostFiles", id = RouteParameter.Optional }
       );

            config.Routes.MapHttpRoute(
           "AddPostVideo",
           "api/v1/Post/AddPostVideo",
           new { controller = "Upload", action = "PostVideos", id = RouteParameter.Optional }
       );
            #endregion

            #region webhtml

            config.Routes.MapHttpRoute(
           "getwebhtml",
           "api/v1/webhtml/GetByPageIdentifier",
           new { controller = "Webhtml", action = "GetByPageIdentifier", id = RouteParameter.Optional }
             );
            #endregion


            #region Brand

            config.Routes.MapHttpRoute(
           "getAllBrand",
           "api/v1/Brand/GetallBrand",
           new { controller = "Brand", action = "GetBrandAll", id = RouteParameter.Optional }
             );


            config.Routes.MapHttpRoute(
           "getBrandProducts",
           "api/v1/Brand/getBrandProducts",
           new { controller = "Brand", action = "GetBrandProducts", id = RouteParameter.Optional }
             );

            #endregion


            #region Categories
            config.Routes.MapHttpRoute(
               "Categories",
               "api/v1/general/categories",
               new { controller = "Category", action = "GetAll", id = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
             "Specific Categories",
             "api/v1/general/categoriesById",
             new { controller = "Category", action = "GetCategoryById", id = RouteParameter.Optional }
         );

            config.Routes.MapHttpRoute(
          "Parent Categoriesproducts",
          "api/v1/general/ParentcategoriesWiseproduct",
          new { controller = "Category", action = "GetParentCategorywiseProduct", id = RouteParameter.Optional }
      );

            config.Routes.MapHttpRoute(
          "singel Categoriesproducts",
          "api/v1/general/GetSingelCategorywiseProduct",
          new { controller = "Category", action = "GetSingelCategorywiseProduct", id = RouteParameter.Optional }
      );

            config.Routes.MapHttpRoute(
      "singel CategoriesService",
      "api/v1/general/GetSingelCategorywiseSerevice",
      new { controller = "Category", action = "GetSingelCategorywiseService", id = RouteParameter.Optional }
  );

            config.Routes.MapHttpRoute(
"Get_SingelCategorywiseOnlyService",
"api/v1/general/GetSingelCategorywiseOnlyService",
new { controller = "Category", action = "GetSingelCategorywiseOnlyService", id = RouteParameter.Optional }
);

            config.Routes.MapHttpRoute(
"GetSingelCategorywiseOnlyTypingAndInsuranceService",
"api/v1/general/GetSingelCategorywiseOnlyTypingAndInsuranceService",
new { controller = "Category", action = "GetSingelCategorywiseOnlyTypingAndService", id = RouteParameter.Optional }
);

            

            #endregion


            #region product




            config.Routes.MapHttpRoute(
           "getProductDetails",
           "api/v1/Product/getProductDetails",
           new { controller = "Product", action = "GetProductDetails", id = RouteParameter.Optional }
             );

            config.Routes.MapHttpRoute(
          "getProductsearch",
          "api/v1/Product/getProductsearch",
          new { controller = "Product", action = "GetProductSearch", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
      "GetBestSellingProducts",
      "api/v1/Product/GetBestSellingProducts",
      new { controller = "Product", action = "GetBestSellingProducts", id = RouteParameter.Optional }
        );


            config.Routes.MapHttpRoute(
      "GetRelatedProducts",
      "api/v1/Product/GetRelatedProducts",
      new { controller = "Product", action = "GetRelatedProducts", id = RouteParameter.Optional }
        );


            config.Routes.MapHttpRoute(
      "AddMoreProducts",
      "api/v1/Product/GetMoreProducts",
      new { controller = "Product", action = "GetMoreProducts", id = RouteParameter.Optional }
        );
            config.Routes.MapHttpRoute(
"GetallProductTags",
"api/v1/Product/GetallProductTags",
new { controller = "Product", action = "GetallProductTags", id = RouteParameter.Optional }
  );
            config.Routes.MapHttpRoute(
"GetallProductBytag",
"api/v1/Product/GetallProductBytag",
new { controller = "Product", action = "GetallProductBytag", id = RouteParameter.Optional }
);


            

            #endregion

            #region CartList
            config.Routes.MapHttpRoute(
                "CartList_add_remove",
                "api/v1/CartList/add-remove",
                new { controller = "Cart", action = "AddOrRemoveCart", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
              "CartList_RemoveCarte",
              "api/v1/CartList/remove/{memberId}",
              new { controller = "Cart", action = "RemoveCart", memberId = RouteParameter.Optional }
          );


            config.Routes.MapHttpRoute(
                "CartList_products",
                "api/v1/CartList/products",
                new { controller = "Cart", action = "GetCartProducts", id = RouteParameter.Optional }
            );


            config.Routes.MapHttpRoute(
              "CartList_Services",
              "api/v1/CartList/GetCartService",
              new { controller = "Cart", action = "GetCartService", id = RouteParameter.Optional }
          );
            
            config.Routes.MapHttpRoute(
             "CartList_productsCount",
             "api/v1/CartList/productsCount",
             new { controller = "Cart", action = "GetCartCount", id = RouteParameter.Optional }
         );


            config.Routes.MapHttpRoute(
             "CartList_add_removeService",
             "api/v1/CartList/add-removeService",
             new { controller = "Cart", action = "AddOrRemoveCartService", id = RouteParameter.Optional }
         );

            config.Routes.MapHttpRoute(
              "CartList_RemoveCarteService",
              "api/v1/CartList/removeService/{memberId}",
              new { controller = "Cart", action = "RemoveCartCartService", memberId = RouteParameter.Optional }
          );

            #endregion

            #region MemberAddress

            config.Routes.MapHttpRoute(
           "InsertMemberAddress",
           "api/v1/MemberAddress/Insert",
           new { controller = "MemberAddress", action = "InsertMemberAddress", id = RouteParameter.Optional }
             );


            config.Routes.MapHttpRoute(
            "UpdateMemberAddress",
            "api/v1/MemberAddress/Update",
            new { controller = "MemberAddress", action = "UpdateMemberAddress", id = RouteParameter.Optional }
              );

            config.Routes.MapHttpRoute(
           "MakeDefaultAddress",
           "api/v1/MemberAddress/MakeDefaultAddress",
           new { controller = "MemberAddress", action = "MakeDefaultAddress", id = RouteParameter.Optional }
             );

            config.Routes.MapHttpRoute(
          "DeleteAddress",
          "api/v1/MemberAddress/DeleteAddress",
          new { controller = "MemberAddress", action = "RemoveAddress", id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
          "GetMemberAddress",
          "api/v1/MemberAddress/GetMemberAddress",
          new { controller = "MemberAddress", action = "GetMemberAddress", id = RouteParameter.Optional }
            );


            #endregion
            #region Country

            config.Routes.MapHttpRoute(
           "GetAllCountries",
           "api/v1/Country/GetAll",
           new { controller = "Country", action = "GetAllCountries", id = RouteParameter.Optional }
             );


            config.Routes.MapHttpRoute(
            "GetCountryById",
            "api/v1/Country/GetById",
            new { controller = "Country", action = "GetCountryById", id = RouteParameter.Optional }
              );

            #endregion

            #region City

            config.Routes.MapHttpRoute(
           "GetAllCities",
           "api/v1/City/GetAll",
           new { controller = "City", action = "GetAllCities", id = RouteParameter.Optional }
             );


            config.Routes.MapHttpRoute(
            "GetCitybyId",
            "api/v1/City/GetById",
            new { controller = "City", action = "GetCitybyId", id = RouteParameter.Optional }
              );

            config.Routes.MapHttpRoute(
           "GetCitiesByCountryId",
           "api/v1/City/GetCitiesByCountryId",
           new { controller = "City", action = "GetCitiesByCountryId", id = RouteParameter.Optional }
             );
            #endregion

            #region PaymentMethod
            config.Routes.MapHttpRoute(
          "PaymentMethod",
          "api/v1/payment/getpaymentmethod",
          new { controller = "PaymentMethod", action = "GetAllPaymentMethod", id = RouteParameter.Optional }
            );
            #endregion

            #region Order Request
            config.Routes.MapHttpRoute(
            "Orders Create",
            "api/v1/Orders/CreateOrders",
            new { controller = "OrderRequest", action = "OrderSubmit", id = RouteParameter.Optional }
        );

            config.Routes.MapHttpRoute(
           "Get Order List",
           "api/v1/Order/getOrderList",
           new { controller = "OrderRequest", action = "getOrdersByMember", id = RouteParameter.Optional }
       );
            config.Routes.MapHttpRoute(
                "Insert Order Request Service",
                "api/v1/Order/InsertOrderRequestService",
                new { controller = "OrderRequest", action = "InsertOrderRequestService", id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
      "getOrderServicesByMember",
      "api/v1/Order/getOrderServicesByMember",
      new { controller = "OrderRequest", action = "getOrderServicesByMember", id = RouteParameter.Optional }
      );

            config.Routes.MapHttpRoute(
            "SearchAllProductAndService",
        "api/v1/Order/SearchAllProductAndService",
        new { controller = "OrderRequest", action = "SearchAllProductAndService", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
            "UpdatePaymentStatusService",
        "api/v1/Order/UpdatePaymentStatusService",
        new { controller = "OrderRequest", action = "UpdatePaymentStatusService", id = RouteParameter.Optional }
            );


            config.Routes.MapHttpRoute(
          "GetProductStatus",
      "api/v1/Order/GetProductStatus",
      new { controller = "OrderRequest", action = "GetProductStatus", id = RouteParameter.Optional }
          );


            

            #endregion

            #region Service
            config.Routes.MapHttpRoute(
            "GetServices",
            "api/v1/Service/GetServices",
            new { controller = "Service", action = "GetServices", id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
           "GetServiceDetailsById",
           "api/v1/Service/GetServiceDetailsById",
           new { controller = "Service", action = "GetServiceDetailsById", id = RouteParameter.Optional }
           );
            config.Routes.MapHttpRoute(
           "GetRelatedServices",
           "api/v1/Service/GetRelatedServices",
           new { controller = "Service", action = "GetRelatedServices", id = RouteParameter.Optional }
           );
            config.Routes.MapHttpRoute(
           "SearchingServiceType",
           "api/v1/Service/SearchingServiceType",
           new { controller = "Service", action = "SearchingServiceType", id = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
      "GetSimilarDestination",
      "api/v1/Service/GetSimilarDestination",
      new { controller = "Service", action = "GetSimilarDestination", id = RouteParameter.Optional }
      );
            config.Routes.MapHttpRoute(
 "GetPackaedges",
 "api/v1/Service/GetPackaedges",
 new { controller = "Service", action = "GetPackaedges", id = RouteParameter.Optional }
 );

            config.Routes.MapHttpRoute(
            "GetLatestProjectForrealEstate",
            "api/v1/Service/GetLatestProjectForrealEstate",
            new { controller = "Service", action = "GetlatestProjectServicerealeastate", id = RouteParameter.Optional }
             );
            config.Routes.MapHttpRoute(
       "GetServiceDetailsrealStateById",
       "api/v1/Service/GetServiceDetailsrealStateById",
       new { controller = "Service", action = "GetServiceDetailsrealStateById", id = RouteParameter.Optional }
        );
            config.Routes.MapHttpRoute(
   "GetFilterResponse",
   "api/v1/Service/GetFilterResponse",
   new { controller = "Service", action = "GetFilterResponse", id = RouteParameter.Optional }
    );

            config.Routes.MapHttpRoute(
"GetFilterWiseServiceRealEstate",
"api/v1/Service/GetFilterWiseServiceRealEstate",
new { controller = "Service", action = "GetFilterWiseServiceRealEstate", id = RouteParameter.Optional }
);

            config.Routes.MapHttpRoute(
"GetlocationWiseRealEstate",
"api/v1/Service/GetlocationWiseRealEstate",
new { controller = "Service", action = "GetlocationWiseRealEstate", id = RouteParameter.Optional }
);

            config.Routes.MapHttpRoute(
"GetFeatureWiseeRealEstate",
"api/v1/Service/GetFeatureWiseeRealEstate",
new { controller = "Service", action = "GetFeatureWiseeRealEstate", id = RouteParameter.Optional }
);


            config.Routes.MapHttpRoute(
"GetFeatureWiseGetServiceAmenitieseRealEstate",
"api/v1/Service/GetServiceAmenities",
new { controller = "Service", action = "GetServiceAmenities", id = RouteParameter.Optional }
);
            config.Routes.MapHttpRoute(
"GetServiceNameTypingAndInsurance",
"api/v1/Service/GetServiceNameTypingAndInsurance",
new { controller = "Service", action = "GetServiceNameTypingAndInsurance", id = RouteParameter.Optional }
);

            config.Routes.MapHttpRoute(
"GetServicesByIdTypingandInsurance",
"api/v1/Service/GetServicesByIdTypingandInsurance",
new { controller = "Service", action = "GetServicesByIdTypingandInsurance", id = RouteParameter.Optional }
);
            

            #endregion

            #region Airport
            config.Routes.MapHttpRoute(
                "GetAllAirports",
                "api/v1/Airport/GetAllAirports",
                new { controller = "Airport", action = "GetAllAirports", id = RouteParameter.Optional }
                );

            config.Routes.MapHttpRoute(
                "GetAirportById",
                "api/v1/Airport/GetAirportById",
                new { controller = "Airport", action = "GetAirportById", id = RouteParameter.Optional }
                );



            #endregion

            #region Vehical Model
            config.Routes.MapHttpRoute(
          "GetVehicalModelbyBrandId",
          "api/v1/VehicalModel/GetVehicalModelbyBrandId",
          new { controller = "MemberVehicalModel", action = "GetVehicalModelbyBrandId", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
          "GetMemberInfobyMemberId",
          "api/v1/VehicalModel/GetMemberInfobyMemberId",
          new { controller = "MemberVehicalModel", action = "GetMemberInfobyMemberId", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
        "CreateMemberVehical",
        "api/v1/VehicalModel/CreateMemberVehical",
        new { controller = "MemberVehicalModel", action = "CreatemembervehicalInfo", id = RouteParameter.Optional }
          );


            #endregion

            #region Offers

            config.Routes.MapHttpRoute(
          "GetTrandingBrandOffers",
          "api/v1/Offers/GetTrandingBrandOffer",
          new { controller = "Offer", action = "GetTrandingBrandOffer", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
         "GetTrandingProductOffers",
         "api/v1/Offers/GetTrandingProductOffer",
         new { controller = "Offer", action = "GetTrandingProductOffer", id = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
      "GetTrandingcategoryOffer",
      "api/v1/Offers/GetTrandingcategoryOffer",
      new { controller = "Offer", action = "GetTrandingcategoryOffer", id = RouteParameter.Optional }
        );

            config.Routes.MapHttpRoute(
    "GetTrandingcategoryOfferServices",
    "api/v1/Offers/GetTrandingcategoryOfferServices",
        new { controller = "Offer", action = "GetTrandingcategoryOfferServices", id = RouteParameter.Optional }
        );
            config.Routes.MapHttpRoute(
      "GetTrandingServiceOffer",
      "api/v1/Offers/GetTrandingServiceOffer",
      new { controller = "Offer", action = "GetTrandingServiceOffer", id = RouteParameter.Optional }
        );


            #endregion

            #region User Report

            config.Routes.MapHttpRoute(
           "SaveQuestion",
           "api/v1/PostuserReport",
           new { controller = "UserReport", action = "SaveQuestion", id = RouteParameter.Optional }
             );


            config.Routes.MapHttpRoute(
            "GetUserReportresponse",
            "api/v1/GetuserReport",
            new { controller = "UserReport", action = "GetUserReportresponse", id = RouteParameter.Optional }
              );

            #endregion

            #region User Review

            config.Routes.MapHttpRoute(
           "AddUserReview",
           "api/v1/UserReviewAdd",
           new { controller = "UserReview", action = "UserReviewAdd", id = RouteParameter.Optional }
             );



            #endregion



            #region Favourite
            config.Routes.MapHttpRoute(
          "AddOrRemoveFavourite",
          "api/v1/Favourite/AddOrRemoveFavourite",
          new { controller = "Favourite", action = "AddOrRemoveFavourite", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
          "getFavouriteProducts",
          "api/v1/Favourite/getFavouriteProducts",
          new { controller = "Favourite", action = "getFavouriteProducts", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
        "getFavouriteService",
        "api/v1/Favourite/getFavouriteService",
        new { controller = "Favourite", action = "getFavouriteService", id = RouteParameter.Optional }
          );


            #endregion


            #region GetDeliverySettings


            config.Routes.MapHttpRoute(
        "GetDeliverySettings",
        "api/v1/DeliverySetting/GetDeliverySettings",
        new { controller = "DeliverySetting", action = "GetDeliverySettings", id = RouteParameter.Optional }
          );


            #endregion

            #region FaqService
            config.Routes.MapHttpRoute(
                "GetAlFAQ",
                "api/v1/faq/getAll",
            new { controller = "FAQ", action = "GetAlFAQ", id = RouteParameter.Optional }
            );
            #endregion

            #region Customer Enquery
            config.Routes.MapHttpRoute(
                "AddEnquery",
                "api/v1/Enquery/AddEnquery",
            new { controller = "CustomerEnquery", action = "AddCustomerenquery", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
               "GetCustomerEnquery",
               "api/v1/Enquery/GetCustomerEnquery",
           new { controller = "CustomerEnquery", action = "GetCustomerEnquery", id = RouteParameter.Optional }
           );
            #endregion

            #region Subscription
            config.Routes.MapHttpRoute(
            "GetSubscription",
            "api/v1/membership/GetSubscription",
        new { controller = "Membership", action = "GetSubscription", id = RouteParameter.Optional }
        );

            config.Routes.MapHttpRoute(
          "CreateSubscription",
          "api/v1/membership/CreateSubscription",
      new { controller = "Membership", action = "CreateSubscription", id = RouteParameter.Optional }
      );



            #endregion

            #region Community Setup
            config.Routes.MapHttpRoute(
                "CommunitySetup",
                "api/v1/community-setup",
            new { controller = "CommunitySetup", action = "Index", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
              "Insert_CommunitySetup",
              "api/v1/insert-community-setup",
          new { controller = "CommunitySetup", action = "Insert", id = RouteParameter.Optional }
          );
            #endregion

            #region push
            config.Routes.MapHttpRoute(
                           "PushNotification",
                           "api/v1/Push",
                           new { controller = "Push", action = "PushSend", id = RouteParameter.Optional }
                       );

            config.Routes.MapHttpRoute(
                          "PushNotificationEmail",
                          "api/v1/PushSendemail",
                          new { controller = "Push", action = "PushSendemail", id = RouteParameter.Optional }
                      );
            config.Routes.MapHttpRoute(
                       "SeenAdminNotification",
                       "api/v1/SeenAdminNotification",
                       new { controller = "Push", action = "SeenAdminNotification", id = RouteParameter.Optional }
                   );
            

            config.Routes.MapHttpRoute(
              "company_profile_Update",
              "company/profile/update",
              new { controller = "Company", action = "UpdateStatus", id = RouteParameter.Optional }
            );

            #endregion



            #region Notifications

            config.Routes.MapHttpRoute(
             "notifications",
             "api/v1/Notification/notifications",
             new { controller = "Notification", action = "NotificationsGet", areaId = RouteParameter.Optional }
           );
            config.Routes.MapHttpRoute(
               "notifications_seen",
               "api/v1/Notification/notifications/seen",
               new { controller = "Notification", action = "NotificationsSeen", areaId = RouteParameter.Optional }
           );
            config.Routes.MapHttpRoute(
               "notifications_received",
               "api/v1/Notification/notifications/received",
               new { controller = "Notification", action = "NotificationsReceived", areaId = RouteParameter.Optional }
           );


            config.Routes.MapHttpRoute(
             "notifications_Clear",
             "api/v1/Notification/notifications/Clear",
             new { controller = "Notification", action = "NotificationsClear", areaId = RouteParameter.Optional }
         );



            #endregion

            config.Routes.MapHttpRoute(
             "ProductTypeGet",
             "api/v1/ProductType/GetAllProductTypes",
             new { controller = "ProductType", action = "GetAllProductTypes", areaId = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
            "PostCourier",
            "api/v1/Courier/PostCourier",
            new { controller = "Courier", action = "PostCourier", areaId = RouteParameter.Optional }
          );

            config.Routes.MapHttpRoute(
         "UpdateCourierStatus",
         "api/v1/Courier/UpdateCourierStatus",
         new { controller = "Courier", action = "UpdateCourierStatus", areaId = RouteParameter.Optional }
       );
            config.Routes.MapHttpRoute(
    "CancelOrderForCourier",
    "api/v1/Courier/CancelOrderForCourier",
    new { controller = "Courier", action = "CancelOrderForCourier", areaId = RouteParameter.Optional }
  );

            
        }

    }
}
