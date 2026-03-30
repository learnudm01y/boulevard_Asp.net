using Boulevard.Contexts;
using Boulevard.Models;
using Boulevard.Service;
using Boulevard.Service.Admin;
using Microsoft.Ajax.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Boulevard.Areas.Admin.Controllers
{
    public class OfferController : Controller
    {
        private readonly OfferAccess _offerAccess;
        private readonly FeatureCategoryAccess _featureCategoryAccess;
        private readonly ProductAccess _productAccess;
        private readonly BrandAccess _brandAccess;
        private readonly CategoryAccess _categoryAccess;
        private readonly ServiceAccess _serviceAccess;
        private readonly OfferDiscountDataAccess _offerDiscountDataAccess;
        public OfferController()
        {
            _offerAccess = new OfferAccess();
            _featureCategoryAccess = new FeatureCategoryAccess();
            _productAccess = new ProductAccess();
            _brandAccess = new BrandAccess();
            _categoryAccess = new CategoryAccess();
            _serviceAccess = new ServiceAccess();
            _offerDiscountDataAccess = new OfferDiscountDataAccess();
        }

        public async Task<ActionResult> Index(string fCatagoryKey)
          {
            ViewBag.FCatagoryKey = fCatagoryKey;
            if (!string.IsNullOrEmpty(fCatagoryKey))
            {
                ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
            }
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> Create(Guid? Key, string fCatagoryKey)
        {
            var db = new BoulevardDbContext();
            if (Key == null || Key == Guid.Empty)
            {
                OfferInformation node = new OfferInformation();
                //ViewBag.Products = new SelectList(await _productAccess.GetAllByFCatagoryKey(fCatagoryKey), "ProductId", "ProductName");
                //ViewBag.Brands = new SelectList(await _brandAccess.GetAllByFeatureCategory(fCatagoryKey), "BrandId", "Title");
                //ViewBag.Categories = new SelectList(await _categoryAccess.GetAllByFeatureCategory(fCatagoryKey), "CategoryId", "CategoryName");
                ViewBag.FeatureCategoryKey = fCatagoryKey;
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {

                    ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                    var featureCategory = await _featureCategoryAccess.GetByKey(Guid.Parse(fCatagoryKey));
                    ViewBag.FeatureType = featureCategory.FeatureType;
                    node.ProductList = await _productAccess.GetAllByFCatagoryKey(fCatagoryKey);
                    node.Categories = await _categoryAccess.GetAllCategoryByfCatagoryKey(fCatagoryKey);
                    node.Brands = await _brandAccess.GetAllByFeatureCategory(fCatagoryKey);
                    node.ServiceTypeList = await _serviceAccess.GetServicesTypeByFeatureCategoryForPackage(fCatagoryKey);
                    //node.ServiceList = await _serviceAccess.GetAllByFeatureCategory(fCatagoryKey);
                    //node.OfferDiscount = await _offerDiscountDataAccess.GetAll();
                }
                return View(node);
            }
            else
            {
                OfferInformation node = await _offerAccess.GetByKey(Key.Value);
                var offerDiscount = await _offerDiscountDataAccess.GetAllByOfferInformationId(node.OfferInformationId);
                if(offerDiscount != null)
                {

                    node.DiscountType = offerDiscount.DiscountType;
                    node.DiscountTypeAr = offerDiscount.DiscountTypeAr;
                    node.DiscountAmount = offerDiscount.DiscountAmount;
                }
                if (node != null)
                {
                    FeatureCategory featureCategory = await _featureCategoryAccess.GetById(node.FeatureCategoryId);
                    ViewBag.FeatureCategoryKey = featureCategory.FeatureCategoryKey;
                    ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(featureCategory.FeatureCategoryKey.ToString());

                    //ViewBag.Products = new SelectList(await _productAccess.GetAllByFCatagoryKey(featureCategory.FeatureCategoryKey.ToString()), "ProductId", "ProductName");
                    //ViewBag.Brands = new SelectList(await _brandAccess.GetAllByFeatureCategory(featureCategory.FeatureCategoryKey.ToString()), "BrandId", "Title");
                    //ViewBag.Categories = new SelectList(await _categoryAccess.GetAllByFeatureCategory(featureCategory.FeatureCategoryKey.ToString()), "CategoryId", "CategoryName");
                    if (node.IsProduct == true)
                    {
                        node.ProductList = await _productAccess.GetAllByFCatagoryKey(featureCategory.FeatureCategoryKey.ToString());
                    }
                    else
                    {
                        node.ProductList = null;


                    }
                    if (node.IsCategory == true)
                    {
                        node.Categories = await _categoryAccess.GetAllCategoryByfCatagoryKey(featureCategory.FeatureCategoryKey.ToString());
                    }
                    else
                    {
                        node.Categories = null;
                    }
                    if (node.IsBrand == true)
                    {
                        node.Brands = await _brandAccess.GetAllByFeatureCategory(featureCategory.FeatureCategoryKey.ToString());
                    }
                    else
                    {
                        node.Brands = null;
                    }
                   
                    node.ServiceList = null;/*await _serviceAccess.GetAllByFeatureCategory(featureCategory.FeatureCategoryKey.ToString())*/
                    if (node.IsService == true)
                    {
                        node.ServiceTypeList = await _serviceAccess.GetServicesTypeByFeatureCategoryForPackage(fCatagoryKey);
                    }
                    else
                    {
                        node.ServiceTypeList = null;
                    }


                    if (node.Brands != null)
                    {
                        foreach (var category in node.Brands)
                        {
                            var brandOffer = db.BrandOffers.Where(s => s.BrandId == category.BrandId && s.OfferInformationId == node.OfferInformationId).FirstOrDefault();
                            if (brandOffer != null)
                            {
                                var isSelected = node.Brands.Where(s => s.BrandId == brandOffer.BrandId).FirstOrDefault();
                                if (isSelected != null)
                                {
                                    category.IsSelected = true;
                                }
                            }
                        }
                    }

                    if (node.Categories != null)
                    {
                        foreach (var category in node.Categories)
                        {
                            var categoryOffers = db.CategoryOffers.Where(s => s.CategoryId == category.CategoryId && s.OfferInformationId == node.OfferInformationId).FirstOrDefault();
                            if (categoryOffers != null)
                            {
                                var isSelected = node.Categories.Where(s => s.CategoryId == category.CategoryId).FirstOrDefault();
                                if (isSelected != null)
                                {
                                    category.IsSelected = true;
                                }
                            }
                        }
                    }

                    if (node.ProductList != null)
                    {
                        foreach (var product in node.ProductList)
                        {
                            var productOffers = db.ProductOffers.Where(s => s.ProductId == product.ProductId && s.OfferInformationId == node.OfferInformationId).FirstOrDefault();
                            if (productOffers != null)
                            {
                                var isSelected = node.Products.Where(s => s.ProductId == productOffers.ProductId).FirstOrDefault();
                                if (isSelected != null)
                                {
                                    product.IsSelected = true;
                                }
                            }
                        }
                    }

                    if (node.ServiceTypeList != null)
                    {
                        foreach (var service in node.ServiceTypeList)
                        {
                            var serviceOffers = db.ServiceOffers.Where(s => s.ServiceId == service.ServiceId && s.OfferInformationId == node.OfferInformationId && s.ServiceTypeId==service.ServiceTypeId).FirstOrDefault();
                            if (serviceOffers != null)
                            {
                                var isSelected = node.ServiceTypeList.Where(s => s.ServiceTypeId == service.ServiceTypeId).FirstOrDefault();
                                if (isSelected != null)
                                {
                                    service.IsSelected = true;
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(featureCategory.FeatureCategoryKey.ToString()))
                    {
                        var fCategory = await _featureCategoryAccess.GetByKey(Guid.Parse(fCatagoryKey));
                        ViewBag.FeatureType = fCategory.FeatureType;
                    }
                    OfferBanner offerBanner = await _offerAccess.GetOfferBannerById(node.OfferInformationId);
                    if (offerBanner != null)
                    {
                        node.BannerImage = offerBanner.BannerImage;
                    }
                }
                return View(node);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(OfferInformation model, HttpPostedFileBase Image)
        {
            var db = new BoulevardDbContext();
            if (model.OfferInformationKey == Guid.Empty)
            {
                var featureCategory = new FeatureCategory();
                if (!string.IsNullOrEmpty(model.FeatureCategoryKey))
                {
                    featureCategory = await _featureCategoryAccess.GetByKey(Guid.Parse(model.FeatureCategoryKey));
                    model.FeatureCategoryId = featureCategory.FeatureCategoryId;
                    model.FeatureType = featureCategory.FeatureType;
                }
                var offerData = await _offerAccess.Insert(model);

                if (model.DiscountType != null && model.DiscountAmount > 0)
                {
                    var offerDiscount = new OfferDiscount();
                    offerDiscount.DiscountType = model.DiscountType;
                    offerDiscount.DiscountTypeAr = model.DiscountTypeAr;
                    offerDiscount.DiscountAmount = model.DiscountAmount;
                    offerDiscount.OfferInformationId = offerData.OfferInformationId;
                    await _offerDiscountDataAccess.Insert(offerDiscount);
                }

                if (Image != null)
                {
                    var offerBanner = new OfferBanner();
                    offerBanner.OfferInformationId = offerData.OfferInformationId;
                    offerBanner.IsFeature = offerData.IsFeature;
                    offerBanner.BannerImage = _offerAccess.UploadImage(Image);
                    await _offerAccess.InsertOfferBanner(offerBanner);
                }
                if (model.IsBrand == true)
                {
                    if (model.Brands != null)
                    {
                        foreach (var brandNode in model.Brands.Where(s => s.IsSelected == true))
                        {
                            var brand = new BrandOffer();
                            brand.BrandId = brandNode.BrandId;
                            brand.OfferInformationId = offerData.OfferInformationId;
                            db.BrandOffers.Add(brand);
                            db.SaveChanges();
                        }
                    }
                }
                if (model.IsCategory == true)
                {
                    if (model.Categories != null)
                    {
                        foreach (var categoryNode in model.Categories.Where(s => s.IsSelected == true))
                        {
                            var category = new CategoryOffer();
                            category.CategoryId = categoryNode.CategoryId;
                            category.OfferInformationId = offerData.OfferInformationId;
                            db.CategoryOffers.Add(category);
                            db.SaveChanges();
                        }
                    }
                }
                if (model.IsProduct == true)
                {
                    if (model.ProductList != null)
                    {
                        foreach (var productNode in model.ProductList.Where(s => s.IsSelected == true))
                        {
                            var product = new ProductOffer();
                            product.ProductId = productNode.ProductId;
                            product.OfferInformationId = offerData.OfferInformationId;
                            product.IsFeature = false;
                            db.ProductOffers.Add(product);
                            db.SaveChanges();
                        }
                    }
                
                }
                if (model.IsService == true)
                {
                    if (model.ServiceTypeList != null)
                    {
                        foreach (var serviceNode in model.ServiceTypeList.Where(s => s.IsSelected == true))
                        {
                            //var service = new ServiceOffers();
                            //service.ServiceId = serviceNode.ServiceId;
                            //service.OfferInformationId = offerData.OfferInformationId;
                            //service.IsFeature = false;
                            ////service.ServiceTypeId = 1;
                            //db.ServiceOffers.Add(service);
                            //db.SaveChanges();

                            var service = new ServiceOffers();
                            service.ServiceId = db.ServiceTypes.Where(s => s.ServiceTypeId == serviceNode.ServiceTypeId).Select(s => s.ServiceId).FirstOrDefault();
                            service.OfferInformationId = offerData.OfferInformationId;
                            service.IsFeature = false;
                            service.ServiceTypeId = service.ServiceTypeId;
                            db.ServiceOffers.Add(service);
                            db.SaveChanges();
                        }
                    }
                   
                }
            }
            else
            {
                var offerNode = await _offerAccess.Update(model);

                if (offerNode != null)
                {
                    var offerDiscountNode = await _offerDiscountDataAccess.GetAllByOfferInformationId(offerNode.OfferInformationId);
                    if (offerDiscountNode != null)
                    {
                        var offerDiscount = new OfferDiscount();
                        offerDiscountNode.DiscountType = model.DiscountType;
                        offerDiscountNode.DiscountTypeAr = model.DiscountTypeAr;
                        offerDiscountNode.DiscountAmount = model.DiscountAmount;
                        offerDiscountNode.OfferInformationId = offerNode.OfferInformationId;
                        await _offerDiscountDataAccess.Update(offerDiscountNode);
                    }
                    else
                    {
                        if (model.DiscountType != null && model.DiscountAmount > 0)
                        {
                            var offerDiscount = new OfferDiscount();
                            offerDiscount.DiscountType = model.DiscountType;
                            offerDiscount.DiscountTypeAr = model.DiscountTypeAr;
                            offerDiscount.DiscountAmount = model.DiscountAmount;
                            offerDiscount.OfferInformationId = offerNode.OfferInformationId;
                            await _offerDiscountDataAccess.Insert(offerDiscount);
                        }
                    }
                    //var db = new BoulevardDbContext();
                    if (Image != null)
                    {
                        var offerDataUpdate = await _offerAccess.GetByKey(model.OfferInformationKey);
                        //var offerBannerData = await _offerAccess.GetOfferBannerById(offerDataUpdate.OfferInformationId);
                        var offerBannerData = db.OfferBanner.Where(s => s.OfferInformationId == offerDataUpdate.OfferInformationId).FirstOrDefault();
                        if (offerBannerData != null)
                        {
                            db.Entry(offerBannerData).State = EntityState.Modified;
                            db.SaveChanges();

                            //var offerBanner = new OfferBanner();
                            //offerBanner.OfferInformationId = offerDataUpdate.OfferInformationId;
                            //offerBanner.IsFeature = offerDataUpdate.IsFeature;
                            offerBannerData.BannerImage = _offerAccess.UploadImage(Image);
                            db.Entry(offerBannerData).State = EntityState.Modified;
                            db.SaveChanges();
                            //await _offerAccess.InsertOfferBanner(offerBanner);
                        }
                    }

                    if (model.IsBrand == true)
                    {
                        if (model.Brands != null)
                        {
                            var brandOffer = db.BrandOffers.Where(s => s.OfferInformationId == offerNode.OfferInformationId).ToList();
                            if (brandOffer != null)
                            {
                                foreach (var data in brandOffer)
                                {
                                    db.BrandOffers.Attach(data);
                                    db.BrandOffers.Remove(data);
                                    db.SaveChanges();
                                }
                            }
                            foreach (var brandNode in model.Brands.Where(s => s.IsSelected == true))
                            {
                                var brand = new BrandOffer();
                                brand.BrandId = brandNode.BrandId;
                                brand.OfferInformationId = offerNode.OfferInformationId;
                                db.BrandOffers.Add(brand);
                                db.SaveChanges();
                            }
                        }
                    }

                    if (model.IsCategory == true)
                    {
                        if (model.Categories != null)
                        {
                            var categoryOffer = db.CategoryOffers.Where(s => s.OfferInformationId == offerNode.OfferInformationId).ToList();
                            if (categoryOffer != null)
                            {
                                foreach (var data in categoryOffer)
                                {
                                    db.CategoryOffers.Attach(data);
                                    db.CategoryOffers.Remove(data);
                                    db.SaveChanges();
                                }
                            }
                            foreach (var categoryNode in model.Categories.Where(s => s.IsSelected == true))
                            {
                                var category = new CategoryOffer();
                                category.CategoryId = categoryNode.CategoryId;
                                category.OfferInformationId = offerNode.OfferInformationId;
                                db.CategoryOffers.Add(category);
                                db.SaveChanges();
                            }
                        }
                    
                    }


                    if (model.IsProduct == true)
                    {
                        if (model.ProductList != null)
                        {
                            var productOffer = db.ProductOffers.Where(s => s.OfferInformationId == offerNode.OfferInformationId).ToList();
                            if (productOffer != null)
                            {
                                foreach (var data in productOffer)
                                {
                                    db.ProductOffers.Attach(data);
                                    db.ProductOffers.Remove(data);
                                    db.SaveChanges();
                                }
                            }
                            foreach (var productNode in model.ProductList.Where(s => s.IsSelected == true))
                            {

                                var product = new ProductOffer();
                                product.ProductId = productNode.ProductId;
                                product.OfferInformationId = offerNode.OfferInformationId;
                                product.IsFeature = false;
                                db.ProductOffers.Add(product);
                                db.SaveChanges();
                            }
                        }
                    }

                    if (model.IsService == true)
                    {
                        if (model.ServiceTypeList != null)
                        {
                            var serviceOffer = db.ServiceOffers.Where(s => s.OfferInformationId == offerNode.OfferInformationId).ToList();
                            if (serviceOffer != null)
                            {
                                foreach (var data in serviceOffer)
                                {
                                    db.ServiceOffers.Attach(data);
                                    db.ServiceOffers.Remove(data);
                                    db.SaveChanges();
                                }
                            }
                            foreach (var serviceNode in model.ServiceTypeList.Where(s => s.IsSelected == true))
                            {
                                var service = new ServiceOffers();
                                service.ServiceId = db.ServiceTypes.Where(s => s.ServiceTypeId == serviceNode.ServiceTypeId).Select(s => s.ServiceId).FirstOrDefault();
                                service.OfferInformationId = offerNode.OfferInformationId;
                                service.IsFeature = false;
                                service.ServiceTypeId = serviceNode.ServiceTypeId;
                                db.ServiceOffers.Add(service);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }

            ////return RedirectToAction("Index", "Offer");

            //if (!string.IsNullOrEmpty(model.FeatureCategoryKey))
            //{
            //    return RedirectToAction("Index", "Offer", new { fCatagoryKey = model.FeatureCategoryKey });
            //}
            //else
            //{
            // return RedirectToAction("Index", "Offer",new { fCatagoryKey  = model.FeatureCategoryKey});

            return RedirectToAction("Index", "Offer", new { fCatagoryKey = model.FeatureCategoryKey });

            //}
        }
        public async Task<bool> Delete(Guid? Key)
        {
            if (Key == null || Key == Guid.Empty)
            {
                return false;
            }
            else
            {
                return await _offerAccess.Delete(Key.Value, 1);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetPagedOffers(string fCatagoryKey, string searchTerm, int page = 1, int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 10) pageSize = 10;
                if (pageSize > 500) pageSize = 500;
                var db = new BoulevardDbContext();
                var query = db.OfferInformations.AsQueryable();
                if (!string.IsNullOrEmpty(fCatagoryKey) && Guid.TryParse(fCatagoryKey, out Guid fcGuid))
                {
                    var fcId = await db.featureCategories
                        .Where(f => f.FeatureCategoryKey == fcGuid)
                        .Select(f => f.FeatureCategoryId)
                        .FirstOrDefaultAsync();
                    if (fcId > 0)
                        query = query.Where(o => o.FeatureCategoryId == fcId);
                }
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(o => o.Title.ToLower().Contains(term));
                }
                var rows = await query
                    .OrderByDescending(o => o.OfferInformationId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(o => new {
                        offerKey = o.OfferInformationKey,
                        title = o.Title,
                        description = o.Description,
                        isService = o.IsService,
                        isProduct = o.IsProduct,
                        isBrand = o.IsBrand,
                        isCategory = o.IsCategory,
                        isTimeLimit = o.IsTimeLimit,
                        isTrending = o.IsTrending,
                        bannerImage = db.OfferBanner.Where(b => b.OfferInformationId == o.OfferInformationId).Select(b => b.BannerImage).FirstOrDefault()
                    })
                    .ToListAsync();
                return Json(new { success = true, rows }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetPagedOffersCount(string fCatagoryKey, string searchTerm)
        {
            try
            {
                var db = new BoulevardDbContext();
                var query = db.OfferInformations.AsQueryable();
                if (!string.IsNullOrEmpty(fCatagoryKey) && Guid.TryParse(fCatagoryKey, out Guid fcGuid))
                {
                    var fcId = await db.featureCategories
                        .Where(f => f.FeatureCategoryKey == fcGuid)
                        .Select(f => f.FeatureCategoryId)
                        .FirstOrDefaultAsync();
                    if (fcId > 0)
                        query = query.Where(o => o.FeatureCategoryId == fcId);
                }
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(o => o.Title.ToLower().Contains(term));
                }
                int total = await query.CountAsync();
                return Json(new { success = true, total }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}