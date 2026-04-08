using Microsoft.SqlServer.Server;
using System.Net;
using System.Web.Mvc;

namespace Boulevard.Areas.Admin
{
  public class AdminAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "Admin";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {

      #region Auth
      context.MapRoute(
        "Admin_login",
        "",
        new { controller = "Auth", action = "Login", id = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_logout",
        "admin/logout",
        new { controller = "Auth", action = "Logout", id = UrlParameter.Optional }
      );

      #endregion


      #region Admin Notification


      context.MapRoute(
        "Admin_Notification",
        "admin/AdminNotification",
        new { controller = "Notification", action = "Index", id = UrlParameter.Optional }
      );


      context.MapRoute(
 "Admin_Notification_ForAdmin",
 "Admin/NotificationListForadmin",
 new { controller = "Notification", action = "GetAdminMessages", id = UrlParameter.Optional }
    );

      #endregion

      #region DashBoard
      context.MapRoute(
          "Admin_Dashboard",
          "admin/dashboard",
          new { controller = "DashBoard", action = "Index", id = UrlParameter.Optional }
        );
      context.MapRoute(
          "Admin_LastMonthProductOrderstData",
          "admin/dashboard/last-month-product-orders-data",
          new { controller = "DashBoard", action = "LastMonthProductOrderstData", id = UrlParameter.Optional }
        );
      context.MapRoute(
          "Admin_LastMonthProductSalesData",
          "admin/dashboard/last-month-product-sales-data",
          new { controller = "DashBoard", action = "LastMonthProductSalesData", id = UrlParameter.Optional }
        );
      context.MapRoute(
          "Admin_LastMonthServiceOrdersData",
          "admin/dashboard/last-month-service-orders-data",
          new { controller = "DashBoard", action = "LastMonthServiceOrdersData", id = UrlParameter.Optional }
        );
      context.MapRoute(
          "Admin_LastMonthServiceSalesData",
          "admin/dashboard/last-month-service-sales-data",
          new { controller = "DashBoard", action = "LastMonthServiceSalesData", id = UrlParameter.Optional }
        );
      context.MapRoute(
          "Admin_GetAllChartsData",
          "admin/dashboard/get-all-charts-data",
          new { controller = "DashBoard", action = "GetAllChartsData", id = UrlParameter.Optional }
        );

      context.MapRoute(
          "Admin_GetDashboardStats",
          "admin/dashboard/get-dashboard-stats",
          new { controller = "DashBoard", action = "GetDashboardStats", id = UrlParameter.Optional }
        );

      #endregion

      #region User

      context.MapRoute(
    "Admin_MoodChange",
    "admin/change-mood",
    new { controller = "User", action = "MoodChange", id = UrlParameter.Optional }
  );
      context.MapRoute(
     "Admin_UserList",
     "admin/User-List",
     new { controller = "User", action = "Index", id = UrlParameter.Optional }
   );

      context.MapRoute(
        "Admin_UserCreateOrEdit",
        "admin/User-CreateOrEdit/{id}",
        new { controller = "User", action = "Create", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_UserDelete",
        "admin/User-Delete/{id}",
        new { controller = "User", action = "Delete", id = UrlParameter.Optional }
      );

      #endregion

      #region Country
      context.MapRoute(
       "Admin_Country",
       "admin/country",
       new { controller = "Country", action = "Index", id = UrlParameter.Optional }
     );

      context.MapRoute(
        "Admin_Country_Create_And_Update",
        "admin/create-update-country/{key}",
        new { controller = "Country", action = "CreateAndUpdate", key = UrlParameter.Optional }
      );

      context.MapRoute(
           "Admin_Country_Delete",
           "admin/country/delete/{id}",
           new { controller = "Country", action = "Delete", id = UrlParameter.Optional }
       );
      #endregion

      #region City
      context.MapRoute(
       "Admin_City",
       "admin/city",
       new { controller = "City", action = "Index", id = UrlParameter.Optional }
     );

      context.MapRoute(
        "Admin_City_Create_And_Update",
        "admin/create-update-city/{key}",
        new { controller = "City", action = "CreateAndUpdate", key = UrlParameter.Optional }
      );

      context.MapRoute(
           "Admin_City_Delete",
           "admin/city/delete/{id}",
           new { controller = "City", action = "Delete", id = UrlParameter.Optional }
       );
      #endregion

      #region WebHtml
      context.MapRoute(
       "Admin_WebHtml",
       "admin/webHtml",
       new { controller = "WebHtml", action = "Index", id = UrlParameter.Optional }
     );

      context.MapRoute(
        "Admin_WebHtml_Create_And_Update",
        "admin/create-update-webHtml/{key}",
        new { controller = "WebHtml", action = "CreateAndUpdate", key = UrlParameter.Optional }
      );

      context.MapRoute(
           "Admin_WebHtml_Delete",
           "admin/webHtml/delete/{id}",
           new { controller = "WebHtml", action = "Delete", id = UrlParameter.Optional }
       );

      //Test
      context.MapRoute(
       "Admin_WebHtmlMotorTest",
       "admin/webHtmlMotorTest",
       new { controller = "WebHtml", action = "IndexMotorTest", id = UrlParameter.Optional }
     );
      context.MapRoute(
        "Admin_WebHtml_Create_And_UpdateMotorTest",
        "admin/create-update-webHtmlMotorTest/{key}",
        new { controller = "WebHtml", action = "CreateAndUpdateMotorTest", key = UrlParameter.Optional }
      );

      #region Raisul
      context.MapRoute(
       "Admin_WebHtmlTestIndex",
       "admin/WebHtmlTestIndex",
       new { controller = "WebHtml", action = "WebHtmlTestIndex", id = UrlParameter.Optional }
     );

      context.MapRoute(
        "Admin_WebHtmlTestAdd",
        "admin/WebHtmlTestAdd",
        new { controller = "WebHtml", action = "WebHtmlTestAdd", key = UrlParameter.Optional }
      );

      context.MapRoute(
           "Admin_WebHtmlTestEdit",
           "admin/WebHtmlTestEdit",
           new { controller = "WebHtml", action = "WebHtmlTestEdit", id = UrlParameter.Optional }
       );
      #endregion
      #endregion

      #region Member
      context.MapRoute(
       "Admin_Member",
       "admin/member",
       new { controller = "Member", action = "Index", id = UrlParameter.Optional }
     );

      context.MapRoute(
        "Admin_Member_Create_And_Update",
        "admin/create-update-member/{key}",
        new { controller = "Member", action = "CreateAndUpdate", key = UrlParameter.Optional }
      );

      context.MapRoute(
           "Admin_Member_Delete",
           "admin/member/delete/{id}",
           new { controller = "Member", action = "Delete", id = UrlParameter.Optional }
       );

      context.MapRoute(
        "Admin_Member_Details",
        "admin/details-member/{key}",
        new { controller = "Member", action = "Details", key = UrlParameter.Optional }
      );


      #endregion

      #region Member Address

      context.MapRoute(
        "Admin_Member_Address_Create_And_Update",
        "admin/create-update-member-address/{key}/{memberId}",
        new { controller = "MemberAddress", action = "CreateAndUpdate", key = UrlParameter.Optional, memberId = UrlParameter.Optional }
      );

      context.MapRoute(
           "Admin_Member_Address_Delete",
           "admin/member/address/delete/{id}",
           new { controller = "MemberAddress", action = "Delete", id = UrlParameter.Optional }
       );

      context.MapRoute(
        "Admin_Get_City_By_CountryId",
        "admin/get/city/by/countryId/{countryId}",
        new { controller = "MemberAddress", action = "GetCityByCountryId", countryId = UrlParameter.Optional }
      );
      #endregion


      #region FeatureCategories

      context.MapRoute(
        "Admin_FeatureCategoryList",
        "admin/FeatureCategory-List",
        new { controller = "FeatureCategory", action = "Index", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_FeatureCategoryCreateOrEdit",
        "admin/FeatureCategory-CreateOrEdit/{id}",
        new { controller = "FeatureCategory", action = "Create", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_FeatureCategoryDelete",
        "admin/FeatureCategory-Delete/{id}",
        new { controller = "FeatureCategory", action = "Delete", id = UrlParameter.Optional }
      );

      #region Raisul
      context.MapRoute(
        "Admin_GroceryCategoryIndex",
        "admin/GroceryCategoryIndex",
        new { controller = "FeatureCategory", action = "GroceryCategoryIndex", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_GroceryCategoryAdd",
        "admin/GroceryCategoryAdd",
        new { controller = "FeatureCategory", action = "GroceryCategoryAdd", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_GroceryCategoryEdit",
        "admin/GroceryCategoryEdit",
        new { controller = "FeatureCategory", action = "GroceryCategoryEdit", id = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_ChocolateAndFlawerCategoryIndex",
        "admin/ChocolateAndFlawerCategoryIndex",
        new { controller = "FeatureCategory", action = "ChocolateAndFlawerCategoryIndex", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ChocolateAndFlawerCategoryAdd",
        "admin/ChocolateAndFlawerCategoryAdd",
        new { controller = "FeatureCategory", action = "ChocolateAndFlawerCategoryAdd", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ChocolateAndFlawerCategoryEdit",
        "admin/ChocolateAndFlawerCategoryEdit",
        new { controller = "FeatureCategory", action = "ChocolateAndFlawerCategoryEdit", id = UrlParameter.Optional }
      );
      #endregion
      #endregion

      #region Brand

      context.MapRoute(
        "Admin_BrandList",
        "admin/Brand-List",
        new { controller = "Brand", action = "Index", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_BrandCreateOrEdit",
        "admin/Brand-CreateOrEdit/{id}",
        new { controller = "Brand", action = "Create", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_BrandDelete",
        "admin/Brand-Delete/{id}",
        new { controller = "Brand", action = "Delete", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_Brand_PagedList",
        "admin/brand-paged",
        new { controller = "Brand", action = "GetPagedBrands", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_Brand_PagedCount",
        "admin/brand-paged-count",
        new { controller = "Brand", action = "GetPagedBrandsCount", id = UrlParameter.Optional }
      );

      #region Raisul
      context.MapRoute(
        "Admin_GroceryBrandTestIndex",
        "admin/GroceryBrandTestIndex",
        new { controller = "Brand", action = "GroceryBrandTestIndex", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_GroceryBrandTestAdd",
        "admin/GroceryBrandTestAdd",
        new { controller = "Brand", action = "GroceryBrandTestAdd", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_GroceryBrandTestEdit",
        "admin/GroceryBrandTestEdit",
        new { controller = "Brand", action = "GroceryBrandTestEdit", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ChocolateAndFlawerBrandTestIndex",
        "admin/ChocolateAndFlawerBrandTestIndex",
        new { controller = "Brand", action = "ChocolateAndFlawerBrandTestIndex", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ChocolateAndFlawerBrandTestAdd",
        "admin/ChocolateAndFlawerBrandTestAdd",
        new { controller = "Brand", action = "ChocolateAndFlawerBrandTestAdd", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ChocolateAndFlawerBrandTestEdit",
        "admin/ChocolateAndFlawerBrandTestEdit",
        new { controller = "Brand", action = "ChocolateAndFlawerBrandTestEdit", id = UrlParameter.Optional }
      );
      #endregion
      #endregion

      #region Category

      context.MapRoute(
        "Admin_CategoryList",
        "admin/Category-List",
        new { controller = "Category", action = "Index", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_CategoryCreateOrEdit",
        "admin/Category-CreateOrEdit/{id}",
        new { controller = "Category", action = "Create", id = UrlParameter.Optional }
      );
      context.MapRoute(
     "Admin_CategoryDelete",
     "admin/Category-Delete/{id}",
     new { controller = "Category", action = "Delete", id = UrlParameter.Optional }
   );

      #endregion

      #region Product

      context.MapRoute(
        "Admin_ProductList",
        "admin/Product-List",
        new { controller = "Product", action = "Index", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_Product_PagedList",
        "admin/product-paged",
        new { controller = "Product", action = "GetPagedProducts", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_Product_PagedCount",
        "admin/product-paged-count",
        new { controller = "Product", action = "GetPagedProductsCount", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ProductCreateOrEdit",
        "admin/Product-CreateOrEdit/{id}",
        new { controller = "Product", action = "Create", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ProductDelete",
        "admin/Product-Delete/{id}",
        new { controller = "Product", action = "Delete", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ProductImageDelete",
        "admin/Product-Image-Delete/{id}",
        new { controller = "Product", action = "DeleteImage", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ProductDetails",
        "admin/Product-Image-Details/{id}",
        new { controller = "Product", action = "Details", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_UpsellProduct",
        "admin/Upsell-Product/{key}",
        new { controller = "Product", action = "UpsellProduct", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_PostUpsell",
        "admin/Post-Upsell",
        new { controller = "Product", action = "PostUpsell", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_CrosssellProduct",
        "admin/Crosssell-Product/{key}",
        new { controller = "Product", action = "CrosssellProduct", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_PostCrosssell",
        "admin/Post-Crosssell",
        new { controller = "Product", action = "PostCrosssell", id = UrlParameter.Optional }
      );
      context.MapRoute(
      "Admin_Excell_Upload",
      "admin/Excell_Upload",
      new { controller = "Product", action = "UploadExcel", id = UrlParameter.Optional }
    );
      context.MapRoute(
     "AdminDownloadExcelFormat",
     "admin/AdminDownloadExcelFormat",
     new { controller = "Product", action = "DownloadExcelFormat", id = UrlParameter.Optional }
   );
      context.MapRoute(
        "Admin_StockLog_Details",
        "admin/stockLog_details/{key}",
        new { controller = "Product", action = "StockLogDetails", id = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_StockIn",
        "admin/stockIn",
        new { controller = "Product", action = "StockIn", id = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_StockOut",
        "admin/stockOut",
        new { controller = "Product", action = "StockOut", id = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_Delete_Product_Price",
        "admin/Delete-Product-Price-Delete/{productPriceId}",
        new { controller = "Product", action = "DeleteProductPrice", productPriceId = UrlParameter.Optional }
      );

      context.MapRoute(
    "Admin_AddBulk",
    "admin/product-bulk",
    new { controller = "Product", action = "AddBulk", productPriceId = UrlParameter.Optional }
  );

      context.MapRoute(
  "Admin_DeleteProduct",
  "admin/product-remove-bulk/{fCatagoryKey}",
  new { controller = "Product", action = "DeleteProduct", fCatagoryKey = UrlParameter.Optional }
  );

      context.MapRoute(
"Admin_UpdateAllTemptoProduct",
"admin/product-update-all-bulk/{fCatagoryKey}",
new { controller = "Product", action = "UpdateAllTemptoProduct", fCatagoryKey = UrlParameter.Optional }
);

      context.MapRoute(
"Product_excel_count",
"admin/temp-excel-count",
new { controller = "Product", action = "ExcelImportCount", id = UrlParameter.Optional }
);



      #region Raisul
      context.MapRoute(
        "Admin_GroceryTestIndex",
        "admin/GroceryTestIndex",
        new { controller = "Product", action = "GroceryTestIndex", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_GroceryTestAdd",
        "admin/GroceryTestAdd",
        new { controller = "Product", action = "GroceryTestAdd", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_GroceryTestEdit",
        "admin/GroceryTestEdit",
        new { controller = "Product", action = "GroceryTestEdit", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_GroceryTestDetails",
        "admin/GroceryTestDetails",
        new { controller = "Product", action = "GroceryTestDetails", id = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_ChocolateAndFlawerTestIndex",
        "admin/ChocolateAndFlawerTestIndex",
        new { controller = "Product", action = "ChocolateAndFlawerTestIndex", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ChocolateAndFlawerTestAdd",
        "admin/ChocolateAndFlawerTestAdd",
        new { controller = "Product", action = "ChocolateAndFlawerTestAdd", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ChocolateAndFlawerTestEdit",
        "admin/ChocolateAndFlawerTestEdit",
        new { controller = "Product", action = "ChocolateAndFlawerTestEdit", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ChocolateAndFlawerTestDetails",
        "admin/ChocolateAndFlawerTestDetails",
        new { controller = "Product", action = "ChocolateAndFlawerTestDetails", id = UrlParameter.Optional }
      );
      #endregion

      #endregion

      #region Service

      context.MapRoute(
        "Admin_ServiceList",
        "admin/Service-List",
        new { controller = "Service", action = "Index", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_Service_PagedList",
        "admin/service-paged",
        new { controller = "Service", action = "GetPagedServices", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_Service_PagedCount",
        "admin/service-paged-count",
        new { controller = "Service", action = "GetPagedServicesCount", id = UrlParameter.Optional }
      );
      context.MapRoute(
    "Admin_ServiceListIndexForChildService",
    "admin/Service-ListIndexForChildService",
    new { controller = "Service", action = "IndexForChildService", id = UrlParameter.Optional }
  );
      context.MapRoute(
        "Admin_Service_Child_PagedList",
        "admin/service-child-paged",
        new { controller = "Service", action = "GetPagedChildServices", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_Service_Child_PagedCount",
        "admin/service-child-paged-count",
        new { controller = "Service", action = "GetPagedChildServicesCount", id = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_ServiceCreateOrEdit",
        "admin/Service-CreateOrEdit/{id}",
        new { controller = "Service", action = "Create", id = UrlParameter.Optional }
      );
      context.MapRoute(
      "Admin_ServiceCreateForTypingAndInsurance",
      "admin/Service-CreateForTypingAndInsurance/{id}",
      new { controller = "Service", action = "CreateForTypingAndInsurance", id = UrlParameter.Optional }
    );

      context.MapRoute(
        "Admin_ServiceDelete",
        "admin/Service-Delete/{id}",
        new { controller = "Service", action = "Delete", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ServiceCitiesByCountry",
        "admin/Service-CitiesByCountry",
        new { controller = "Service", action = "GetCityByCountryId", id = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_ServiceImageDelete",
        "admin/Service-Image-Delete/{id}",
        new { controller = "Service", action = "DeleteImage", id = UrlParameter.Optional }
      );

      //test
      context.MapRoute(
        "Admin_ServiceListMotorTest",
        "admin/Service-ListMotorTest",
        new { controller = "Service", action = "MotorServiceIndex", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ServiceCreateOrEditMotorTest",
        "admin/Service-CreateOrEditMotorTest/{id}",
        new { controller = "Service", action = "CreateMotorTest", id = UrlParameter.Optional }
      );

      context.MapRoute(
"Admin_AddServiceBulk",
"admin/service-bulk",
new { controller = "Service", action = "AddServiceBulk", productPriceId = UrlParameter.Optional }
);


      context.MapRoute(
  "Admin_DeleteService",
  "admin/service-remove-bulk/{fCatagoryKey}",
  new { controller = "Service", action = "DeleteService", fCatagoryKey = UrlParameter.Optional }
  );

      context.MapRoute(
"Admin_UpdateAllTemptoService",
"admin/service-update-all-bulk/{fCatagoryKey}",
new { controller = "Service", action = "UpdateAllTemptoService", fCatagoryKey = UrlParameter.Optional }
);

      context.MapRoute(
"Service_excel_count",
"admin/temp-excel-count-service",
new { controller = "Service", action = "ExcelImportCount", id = UrlParameter.Optional }
);


      #endregion

      #region OrderRequestProduct

      context.MapRoute(
        "Admin_Order_Request_Product_List",
        "admin/order/request/product/list",
        new { controller = "OrderRequestProduct", action = "Index", id = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_Update_Status",
        "admin/update/status",
        new { controller = "OrderRequestProduct", action = "UpdateStatus", id = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_Order_Request_Product_Details",
        "admin/order/request/product/details/{id}",
        new { controller = "OrderRequestProduct", action = "Details", id = UrlParameter.Optional }
      );
      context.MapRoute(
     "Admin_Order_Request_GroceryOrder",
     "admin/order/request/GroceryOrderList",
     new { controller = "OrderRequestProduct", action = "GroceryOrderList", id = UrlParameter.Optional }
   );

      context.MapRoute(
   "Admin_Order_Request_GroceryOrderDetails",
   "admin/order/request/GroceryOrderDetails/{id}",
   new { controller = "OrderRequestProduct", action = "GroceryOrderDetails", id = UrlParameter.Optional }
 );

      context.MapRoute(
   "Admin_Order_Request_FlowersOrder",
   "admin/order/request/FlowersandChocolatesOrderList",
   new { controller = "OrderRequestProduct", action = "FlowersandChocolatesOrderList", id = UrlParameter.Optional }
 );

      context.MapRoute(
   "Admin_Order_Request_FlowersandChocolatesOrderDetails",
   "admin/order/request/FlowersandChocolatesOrderDetails",
   new { controller = "OrderRequestProduct", action = "FlowersandChocolatesOrderDetails", id = UrlParameter.Optional }
 );

      context.MapRoute(
       "Admin_Order_CreateInvoice",
       "admin/order/create-Invvoice",
       new { controller = "OrderRequestProduct", action = "CreateInvoice", id = UrlParameter.Optional }
     );


      context.MapRoute(
     "Admin_SearchMember",
     "admin/order/search-member/{term}",
     new { controller = "OrderRequestProduct", action = "SearchMember", term = UrlParameter.Optional }
   );

      context.MapRoute(
    "Admin_GetMemberAddresses",
    "admin/order/search-member-address/{memberId}",
    new { controller = "OrderRequestProduct", action = "GetMemberAddresses", memberId = UrlParameter.Optional }
  );


      context.MapRoute(
     "Admin_GetSearchProduct",
     "admin/order/search-product/{memberId}",
     new { controller = "OrderRequestProduct", action = "SearchProduct", memberId = UrlParameter.Optional }
   );

      context.MapRoute(
   "Admin_SaveAddress",
   "admin/order/save-address",
   new { controller = "OrderRequestProduct", action = "SaveAddress", memberId = UrlParameter.Optional }
 );


      context.MapRoute(
"Admin_SaveMember",
"admin/order/save-member",
new { controller = "OrderRequestProduct", action = "SaveMember", memberId = UrlParameter.Optional }
);



      context.MapRoute(
    "Admin_Order_Request_Product_Invoice",
    "admin/order/request/product/invoice/{id}",
    new { controller = "OrderRequestProduct", action = "Invoice", id = UrlParameter.Optional }
  );

      context.MapRoute(
"Admin_Order_Request_Product_DelivaryShip",
"admin/order/request/product/delivary-ship/{id}",
new { controller = "OrderRequestProduct", action = "DelivaryShip", id = UrlParameter.Optional }
);

      context.MapRoute(
"Admin_Order_Request_Product_TriggerAction",
"admin/order/request/product/triggeraction",
new { controller = "OrderRequestProduct", action = "TriggerAction", id = UrlParameter.Optional }
);

      context.MapRoute(
"Admin_Order_Request_Product_PagedOrders",
"admin/order/request/product/paged-orders",
new { controller = "OrderRequestProduct", action = "GetPagedOrders", id = UrlParameter.Optional }
);

      context.MapRoute(
"Admin_Order_Request_Product_PagedOrdersCount",
"admin/order/request/product/paged-orders-count",
new { controller = "OrderRequestProduct", action = "GetPagedOrdersCount", id = UrlParameter.Optional }
);



      #endregion

      #region Service Amenity

      context.MapRoute(
        "Admin_ServiceFacilitiesList",
        "admin/ServiceFacilities-List",
        new { controller = "ServiceAmenity", action = "IndexForPackage", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ServiceFacilitiesCreateOrEdit",
        "admin/ServiceFacilities-CreateOrEdit/{id}",
        new { controller = "ServiceAmenity", action = "CreateAmenityForPackage", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ServiceAmenityList",
        "admin/ServiceAmenity-List",
        new { controller = "ServiceAmenity", action = "Index", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ServiceAmenityCreateOrEdit",
        "admin/ServiceAmenity-CreateOrEdit/{id}",
        new { controller = "ServiceAmenity", action = "Create", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ServiceAmenityDelete",
        "admin/ServiceAmenity-Delete/{id}",
        new { controller = "ServiceAmenity", action = "Delete", id = UrlParameter.Optional }
      );
      //Test
      context.MapRoute(
        "Admin_ServiceAmenityListMotorTest",
        "admin/ServiceAmenity-ListMotorTest",
        new { controller = "ServiceAmenity", action = "IndexMotorTest", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ServiceAmenityCreateOrEditMotorTest",
        "admin/ServiceAmenity-CreateOrEditMotorTest/{id}",
        new { controller = "ServiceAmenity", action = "CreateMotorTest", id = UrlParameter.Optional }
      );



      #endregion

      #region Service LandMark

      context.MapRoute(
        "Admin_ServiceLandmarkList",
        "admin/ServiceLandmark-List",
        new { controller = "ServiceLandmark", action = "Index", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ServiceLandmarkCreateOrEdit",
        "admin/ServiceLandmark-CreateOrEdit/{id}",
        new { controller = "ServiceLandmark", action = "Create", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ServiceLandmarkDelete",
        "admin/ServiceLandmark-Delete/{id}",
        new { controller = "ServiceLandmark", action = "Delete", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ServiceLandmark_PagedList",
        "admin/servicelandmark-paged",
        new { controller = "ServiceLandmark", action = "GetPagedServiceLandmarks", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ServiceLandmark_PagedCount",
        "admin/servicelandmark-paged-count",
        new { controller = "ServiceLandmark", action = "GetPagedServiceLandmarksCount", id = UrlParameter.Optional }
      );
      #endregion

      #region Service Type

      context.MapRoute(
        "Admin_ServiceTypeList",
        "admin/ServiceType-List",
        new { controller = "ServiceType", action = "Index", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ServiceTypeCreateOrEdit",
        "admin/ServiceType-CreateOrEdit/{id}",
        new { controller = "ServiceType", action = "Create", id = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_ServiceTypeListV2",
        "admin/serviceType-list-v2",
        new { controller = "ServiceType", action = "IndexV2", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ServiceTypeCreateOrEditV2",
        "admin/ServiceType-CreateOrEdit-V2/{id}",
        new { controller = "ServiceType", action = "CreateV2", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ServiceTypeDelete",
        "admin/ServiceType-Delete/{id}",
        new { controller = "ServiceType", action = "Delete", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ServiceTypeImageDelete",
        "admin/ServiceType-Image-Delete/{id}",
        new { controller = "ServiceType", action = "DeleteImage", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ServiceType_PagedList",
        "admin/servicetype-paged",
        new { controller = "ServiceType", action = "GetPagedServiceTypes", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ServiceType_PagedCount",
        "admin/servicetype-paged-count",
        new { controller = "ServiceType", action = "GetPagedServiceTypesCount", id = UrlParameter.Optional }
      );

      //test
      context.MapRoute(
       "Admin_ServiceTypeListMotorTest",
       "admin/ServiceType-ListMotorTest",
       new { controller = "ServiceType", action = "IndexMotorTest", id = UrlParameter.Optional }
          );
      context.MapRoute(
        "Admin_ServiceTypeCreateOrEditMotorTest",
        "admin/ServiceType-CreateOrEditMotorTest/{id}",
        new { controller = "ServiceType", action = "CreateMotorTest", id = UrlParameter.Optional }
          );

      context.MapRoute(
      "Admin_ServiceType_bulk",
      "admin/service-type-bulk",
      new { controller = "ServiceType", action = "AddServiceTypeBulk", productPriceId = UrlParameter.Optional }
    );

      context.MapRoute(
      "ServiceType_excel_count",
      "admin/temp-excel-count-servicetype",
      new { controller = "ServiceType", action = "ExcelImportCount", id = UrlParameter.Optional }
      );


      context.MapRoute(
  "Admin_UpdateAllTemptoServiceType",
  "admin/servicetype-update-all-bulk/{fCatagoryKey}",
  new { controller = "ServiceType", action = "UpdateAllTemptoServiceType", fCatagoryKey = UrlParameter.Optional }
  );

      context.MapRoute(
      "Admin_DeleteServiceType",
      "admin/servicetype-remove-bulk/{fCatagoryKey}",
      new { controller = "ServiceType", action = "DeleteServiceType", fCatagoryKey = UrlParameter.Optional }
      );



      #endregion

      #region Order Status
      context.MapRoute(
       "Admin_Order_Status",
       "admin/order/status",
       new { controller = "OrderStatus", action = "Index", id = UrlParameter.Optional }
     );

      context.MapRoute(
        "Admin_Order_Status_Create_And_Update",
        "admin/create-update-order-status/{key}",
        new { controller = "OrderStatus", action = "CreateAndUpdate", key = UrlParameter.Optional }
      );

      context.MapRoute(
           "Admin_Order_Status_Delete",
           "admin/order/status/delete/{id}",
           new { controller = "OrderStatus", action = "Delete", id = UrlParameter.Optional }
       );
      #endregion

      #region Setting
      context.MapRoute(
        "Admin_Setting",
        "admin/setting/list",
        new { controller = "Setting", action = "Index", id = UrlParameter.Optional }
      );
      #endregion

      #region Role

      context.MapRoute(
        "Admin_RoleList",
        "admin/Role-List",
        new { controller = "Role", action = "Index", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_RoleCreateOrEdit",
        "admin/Role-CreateOrEdit/{id}",
        new { controller = "Role", action = "Create", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_RoleDelete",
        "admin/Role-Delete/{id}",
        new { controller = "Role", action = "Delete", id = UrlParameter.Optional }
      );
      #endregion

      #region Order Request Service

      context.MapRoute(
        "Admin_Order_Request_Service_List",
        "admin/order-service-list",
        new { controller = "OrderRequestService", action = "Index", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_Order_Service_Details",
        "admin/order-service-details/{id}",
        new { controller = "OrderRequestService", action = "Details", id = UrlParameter.Optional }
      );

      context.MapRoute(
     "Admin_Order_MotorsIndex",
     "admin/MotorsIndex",
     new { controller = "OrderRequestService", action = "MotorsIndex", id = UrlParameter.Optional }
   );

      context.MapRoute(
      "Admin_Order_MotorsDetails",
      "admin/order-service-MotorsDetails",
      new { controller = "OrderRequestService", action = "MotorsDetails", id = UrlParameter.Optional }
    );

      context.MapRoute(
        "Admin_Order_SalonIndex",
        "admin/SalonIndex",
        new { controller = "OrderRequestService", action = "SalonIndex", id = UrlParameter.Optional }
      );

      context.MapRoute(
       "Admin_Order_SalonDetails",
       "admin/order-service-SalonDetails",
        new { controller = "OrderRequestService", action = "SalonDetails", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_Order_MedicalIndex",
        "admin/MedicalIndex",
        new { controller = "OrderRequestService", action = "MedicalIndex", id = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_Order_MedicalDetails",
        "admin/order-service-MedicalDetails",
        new { controller = "OrderRequestService", action = "MedicalDetails", id = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_Order_LaundryIndex",
        "admin/LaundryIndex",
        new { controller = "OrderRequestService", action = "LaundryIndex", id = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_Order_LaundryDetails",
        "admin/order-service-LaundryDetails",
        new { controller = "OrderRequestService", action = "LaundryDetails", id = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_Order_PhotographyIndex",
        "admin/PhotographyIndex",
        new { controller = "OrderRequestService", action = "PhotographyIndex", id = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_Order_PhotographyDetails",
        "admin/order-service-PhotographyDetails",
        new { controller = "OrderRequestService", action = "PhotographyDetails", id = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_Order_Service_Approved",
        "admin/order-service-approved/{id}",
        new { controller = "OrderRequestService", action = "Approved", id = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_Order_Service_Quotation",
        "admin/order-service-quotation/{orderRequestServiceId}",
        new { controller = "OrderRequestService", action = "Quotation", orderRequestServiceId = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_Order_Service_Quotation_For_Package",
        "admin/order-service-quotation-for-package/{orderRequestServiceId}",
        new { controller = "OrderRequestService", action = "QuotationForPackage", orderRequestServiceId = UrlParameter.Optional }
      );

      context.MapRoute(
    "Admin_Order_Service_Invoice",
    "admin/order-service-invoice/{id}",
    new { controller = "OrderRequestService", action = "Invoice", id = UrlParameter.Optional }
  );

      context.MapRoute(
    "Admin_Order_Service_DelivaryShip",
    "admin/order-service-delivaryslip/{id}",
    new { controller = "OrderRequestService", action = "DelivaryShip", id = UrlParameter.Optional }
  );

      context.MapRoute(
    "Admin_Order_Service_CreateInvoice",
    "admin/order-service-createInvoice/{fCatagoryKey}",
    new { controller = "OrderRequestService", action = "CreateInvoice", fCatagoryKey = UrlParameter.Optional }
  );
      context.MapRoute(
    "Admin_Order_Service_PagedOrders",
    "admin/order-service-paged",
    new { controller = "OrderRequestService", action = "GetPagedServiceOrders", id = UrlParameter.Optional }
  );
      context.MapRoute(
    "Admin_Order_Service_PagedOrdersCount",
    "admin/order-service-paged-count",
    new { controller = "OrderRequestService", action = "GetPagedServiceOrdersCount", id = UrlParameter.Optional }
  );
      #endregion

      #region Vehical Model
      context.MapRoute(
        "Admin_VehicalModelList",
        "admin/VehicalModel-List",
        new { controller = "VehicalModel", action = "Index", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_VehicalModelCreateAndUpdate",
        "admin/VehicalModel-CreateAndUpdate/{id}",
        new { controller = "VehicalModel", action = "CreateAndUpdate", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_VehicalModelDelete",
        "admin/VehicalModel-Delete/{id}",
        new { controller = "VehicalModel", action = "Delete", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_VehicalModel_PagedList",
        "admin/vehicalmodel-paged",
        new { controller = "VehicalModel", action = "GetPagedVehicalModels", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_VehicalModel_PagedCount",
        "admin/vehicalmodel-paged-count",
        new { controller = "VehicalModel", action = "GetPagedVehicalModelsCount", id = UrlParameter.Optional }
      );
      #endregion

      #region Offer
      context.MapRoute(
        "Admin_OfferList",
        "admin/Offer-List/{id}",
        new { controller = "Offer", action = "Index", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_OfferCreateAndUpdate",
        "admin/Offer-CreateAndUpdate/{id}",
        new { controller = "Offer", action = "Create", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_OfferDelete",
        "admin/Offer-Delete/{id}",
        new { controller = "Offer", action = "Delete", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_Offer_PagedList",
        "admin/offer-paged",
        new { controller = "Offer", action = "GetPagedOffers", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_Offer_PagedCount",
        "admin/offer-paged-count",
        new { controller = "Offer", action = "GetPagedOffersCount", id = UrlParameter.Optional }
      );
      #endregion

      #region Common Product Tag
      context.MapRoute(
        "Admin_Common_Product_Tag_List",
        "admin/common/product/tag/list",
        new { controller = "CommonProductTag", action = "Index", id = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_CommonProductTag",
        "admin/CommonProductTag-Create",
        new { controller = "CommonProductTag", action = "AddCommonProductTag", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_CommonProductTagDelete",
        "admin/CommonProductTag-Delete/{id}",
        new { controller = "CommonProductTag", action = "Delete", id = UrlParameter.Optional }
      );
      #endregion

      #region Customer Enquery
      context.MapRoute(
        "Admin_Customer_Enquery_List",
        "admin/customer/enquery/list",
        new { controller = "CustomerEnquery", action = "Index", id = UrlParameter.Optional }
      );
      #endregion

      #region MemberShip Discount Category

      context.MapRoute(
        "Admin_MemberShipDiscountCategory_List",
        "admin/MemberShipDiscountCategory-List",
        new { controller = "MemberShipDiscountCategory", action = "Index", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_MemberShipDiscountCategory_CreateOrEdit",
        "admin/MemberShipDiscountCategory-CreateOrEdit/{modelId}",
        new { controller = "MemberShipDiscountCategory", action = "CreateAndUpdate", modelId = UrlParameter.Optional }
      );
      #endregion

      #region MemberShip

      context.MapRoute(
        "Admin_MemberShip_List",
        "admin/MemberShip-List",
        new { controller = "MemberShip", action = "Index", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_MemberShip_CreateOrEdit",
        "admin/MemberShip-CreateOrEdit/{key}",
        new { controller = "MemberShip", action = "CreateAndUpdate", key = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_Member_Ship",
        "admin/member-ship-delete/{key}",
        new { controller = "MemberShip", action = "Delete", key = UrlParameter.Optional }
      );
      #endregion

      #region FaqService

      context.MapRoute(
        "Admin_FaqService_List_All",
        "admin/FaqService-List-All",
        new { controller = "FaqService", action = "IndexForAll", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_FaqService_List",
        "admin/FaqService-List",
        new { controller = "FaqService", action = "Index", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_FaqService_CreateAndUpdate",
        "admin/FaqService-CreateAndUpdate/{key}",
        new { controller = "FaqService", action = "CreateAndUpdate", key = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_FaqService_CreateAndUpdateForAll",
        "admin/FaqService-CreateAndUpdateForAll/{id}",
        new { controller = "FaqService", action = "CreateAndUpdateForAll", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_FaqService_Delete",
        "admin/FaqService-Delete/{Key}",
        new { controller = "FaqService", action = "Delete", Key = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_FaqService_PagedList",
        "admin/faqservice-paged",
        new { controller = "FaqService", action = "GetPagedFaqServices", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_FaqService_PagedCount",
        "admin/faqservice-paged-count",
        new { controller = "FaqService", action = "GetPagedFaqServicesCount", id = UrlParameter.Optional }
      );
      #endregion

      #region Report and Help
      context.MapRoute(
       "Admin_Reportandhelp",
       "admin/GetReportandhelp",
       new { controller = "UserReport", action = "GetReportandhelp", id = UrlParameter.Optional }
     );
      context.MapRoute(
        "Admin_ReportandhelpDetails",
        "admin/ReportandhelpDetails/{reportId}",
        new { controller = "UserReport", action = "ReportandhelpDetails", reportId = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_PostResponse",
        "admin/PostResponse/{userReportId}/{responce}",
        new { controller = "UserReport", action = "PostResponse", userReportId = UrlParameter.Optional, responce = UrlParameter.Optional }
      );
      #endregion

      #region Package
      context.MapRoute(
       "Admin_Package_List",
       "admin/package/list",
       new { controller = "Package", action = "Index", id = UrlParameter.Optional }
     );
      context.MapRoute(
        "Admin_Package_CreateOrUpdate",
        "admin/package/createorupdate/{Key}",
        new { controller = "Package", action = "CreatePackage", Key = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_Package_Delete",
        "admin/Package-Delete/{Key}",
        new { controller = "Package", action = "Delete", Key = UrlParameter.Optional }
      );
      #endregion

      #region Airport

      context.MapRoute(
        "Admin_Airport_List",
        "admin/Airport-List",
        new { controller = "Airport", action = "Index", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_Airport_CreateOrEdit",
        "admin/airport-createOrEdit/{key}",
        new { controller = "Airport", action = "CreateAndUpdate", key = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_Airport",
        "admin/airport-delete/{key}",
        new { controller = "Airport", action = "Delete", key = UrlParameter.Optional }
      );
      #endregion

      #region Property Information
      context.MapRoute(
       "Admin_Property_Information",
       "admin/property/information",
       new { controller = "PropertyInformation", action = "Index", id = UrlParameter.Optional }
     );

      context.MapRoute(
        "Admin_Property_Information_Create_And_Update",
        "admin/create-update-property-information/{key}",
        new { controller = "PropertyInformation", action = "CreateAndUpdate", key = UrlParameter.Optional }
      );

      context.MapRoute(
           "Admin_PropertyInformation_Delete",
           "admin/property/information/delete/{id}",
           new { controller = "PropertyInformation", action = "Delete", id = UrlParameter.Optional }
       );

      context.MapRoute(
        "Admin_ServiceTypeFile_Delete",
        "admin/ServiceTypeFile-Delete/{id}",
        new { controller = "PropertyInformation", action = "DeleteImage", id = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_Get_Cities_By_CountryId",
        "admin/get/cities/by/countryId",
        new { controller = "PropertyInformation", action = "GetCitiesByCountryId", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_Property_PagedList",
        "admin/property-paged",
        new { controller = "PropertyInformation", action = "GetPagedPropertyInformations", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_Property_PagedCount",
        "admin/property-paged-count",
        new { controller = "PropertyInformation", action = "GetPagedPropertyInformationsCount", id = UrlParameter.Optional }
      );
      #endregion

      #region Service Type Amenity
      context.MapRoute(
       "Admin_Service_Type_Amenity",
       "admin/service/type/amenity",
       new { controller = "ServiceTypeAmenity", action = "Index", id = UrlParameter.Optional }
     );

      context.MapRoute(
        "Admin_Service_Type_Amenity_Create_And_Update",
        "admin/create-update-service/type/amenity/{key}",
        new { controller = "ServiceTypeAmenity", action = "CreateAndUpdate", key = UrlParameter.Optional }
      );

      context.MapRoute(
           "Admin_Service_Type_Amenity_Delete",
           "admin/service/type/amenity/delete/{id}",
           new { controller = "ServiceTypeAmenity", action = "Delete", id = UrlParameter.Optional }
       );
      context.MapRoute(
        "Admin_ServiceTypeAmenity_PagedList",
        "admin/service-type-amenity-paged",
        new { controller = "ServiceTypeAmenity", action = "GetPagedServiceTypeAmenities", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ServiceTypeAmenity_PagedCount",
        "admin/service-type-amenity-paged-count",
        new { controller = "ServiceTypeAmenity", action = "GetPagedServiceTypeAmenitiesCount", id = UrlParameter.Optional }
      );

      #endregion


      //test
      context.MapRoute(
        "Admin_SalonServiceIndex",
        "admin/Service-SalonServiceIndex",
        new { controller = "Salon", action = "SalonServiceIndex", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ServiceSalonServiceCreate",
        "admin/Service-SalonServiceCreate/{id}",
        new { controller = "Salon", action = "SalonServiceCreate", id = UrlParameter.Optional }
      );

      //test
      context.MapRoute(
        "Admin_SalonServiceTypeIndex",
        "admin/Service-SalonServiceTypeIndex",
        new { controller = "Salon", action = "SalonServiceTypeIndex", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_ServiceSalonServiceTypeCreate",
        "admin/Service-SalonServiceTypeCreate/{id}",
        new { controller = "Salon", action = "SalonServiceTypeCreate", id = UrlParameter.Optional }
      );
      //test
      context.MapRoute(
        "Admin_SalonCategoryIndex",
        "admin/Service-SalonCategoryIndex",
        new { controller = "Salon", action = "SalonCategoryIndex", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_SalonCategoryCreate",
        "admin/Service-SalonCategoryCreate/{id}",
        new { controller = "Salon", action = "SalonCategoryCreate", id = UrlParameter.Optional }
      );
      //test
      context.MapRoute(
        "Admin_SalonBannerIndex",
        "admin/Service-SalonBannerIndex",
        new { controller = "Salon", action = "SalonBannerIndex", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_SalonBannerCreate",
        "admin/Service-SalonBannerCreate/{id}",
        new { controller = "Salon", action = "SalonBannerCreate", id = UrlParameter.Optional }
      );

      #region Social Impact Tracker

      context.MapRoute(
        "Admin_SocialImpactTracker",
        "admin/social-impact-tracker",
        new { controller = "SocialImpactTracker", action = "Index", id = UrlParameter.Optional }
      );

      context.MapRoute(
        "Admin_SocialImpact_UpdateCommission",
        "admin/social-impact-tracker/update-commission",
        new { controller = "SocialImpactTracker", action = "UpdateCommission", id = UrlParameter.Optional }
      );

      #endregion

      #region Delivery Management

      context.MapRoute(
        "Admin_DeliveryManagement",
        "admin/delivery-management",
        new { controller = "DeliveryManagement", action = "Index", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_DeliveryManagement_GetMerchants",
        "admin/delivery-management/merchants",
        new { controller = "DeliveryManagement", action = "GetMerchants", id = UrlParameter.Optional }
      );
      context.MapRoute(
        "Admin_DeliveryManagement_Toggle",
        "admin/delivery-management/toggle",
        new { controller = "DeliveryManagement", action = "ToggleDelivery" }
      );
      context.MapRoute(
        "Admin_DeliveryManagement_Delete",
        "admin/delivery-management/delete",
        new { controller = "DeliveryManagement", action = "DeleteMerchant" }
      );

      #endregion


    }
  }
}