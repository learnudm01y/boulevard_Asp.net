using Boulevard.Helper;
using Boulevard.Models;
using Boulevard.Service;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Boulevard.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {

        private CategoryAccess _categoryAccess;
        private FeatureCategoryAccess _featureCategoryAccess;

        public CategoryController()
        {
            _categoryAccess = new CategoryAccess();
            _featureCategoryAccess = new FeatureCategoryAccess();

        }
        public async Task<ActionResult> Index(string key, bool? isChild, bool? deleteRequest, string fCatagoryKey)
        {
            try
            {
                Category category = new Category();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    ViewBag.featureCategory = new SelectList(await _featureCategoryAccess.GetAllByFCatagoryKey(fCatagoryKey), "FeatureCategoryId", "Name");
                    IEnumerable<Category> result = await _categoryAccess.GetAllByFeatureCategory(fCatagoryKey);
                    List<Category> categoryTree = await _categoryAccess.GetParentChildWiseFCategories(result.ToList(), fCatagoryKey);
                    category.CategoryTree = categoryTree;
                    ViewBag.FeatureCategoryKey = fCatagoryKey.ToString();
                    if (!string.IsNullOrEmpty(fCatagoryKey))
                    {
                        ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                    }
                    if (!string.IsNullOrEmpty(key))
                    {
                        Category selected = await _categoryAccess.GetByKey(new Guid(key));
                        if (deleteRequest.HasValue && deleteRequest.Value)
                        {
                            selected.Status = "Deleted";
                            await _categoryAccess.Delete(selected.CategoryKey, 1);
                            return RedirectToAction("Index", "Category", new { fCatagoryKey = fCatagoryKey });

                        }
                        selected.CategoryTree = categoryTree;
                        ViewBag.categoryId = selected.CategoryId;
                        if (isChild.HasValue && isChild.Value)
                        {
                            Category Clild = new Category();
                            Clild.ParentId = (int)selected.CategoryId;
                            Clild.CategoryTree = categoryTree;
                            return View(Clild);
                        }
                        else
                        {

                            return View(selected);
                        }
                    }
                }
                else
                {
                    ViewBag.featureCategory = new SelectList(await _featureCategoryAccess.GetAll(), "FeatureCategoryId", "Name");
                    IEnumerable<Category> result = await _categoryAccess.GetAll();
                    List<Category> categoryTree = await _categoryAccess.GetParentChildWiseCategories(result.ToList());
                    category.CategoryTree = categoryTree;

                    if (!string.IsNullOrEmpty(key))
                    {
                        Category selected = await _categoryAccess.GetByKey(new Guid(key));
                        if (deleteRequest.HasValue && deleteRequest.Value)
                        {
                            selected.Status = "Deleted";
                            await _categoryAccess.Delete(selected.CategoryKey, 1);
                            return RedirectToAction("Index", "Category", new { fCatagoryKey = fCatagoryKey });

                        }
                        selected.CategoryTree = categoryTree;
                        ViewBag.categoryId = selected.CategoryId;
                        if (isChild.HasValue && isChild.Value)
                        {
                            Category Clild = new Category();
                            Clild.ParentId = (int)selected.CategoryId;
                            Clild.CategoryTree = categoryTree;
                            return View(Clild);
                        }
                        else
                        {

                            return View(selected);
                        }
                    }
                }

                return View(category);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return View(new Brand());
            }

        }
        [HttpPost]
        public async Task<ActionResult> Create(Category categoryViewModel, HttpPostedFileBase Image, HttpPostedFileBase Icon)
        {
            try
            {
                if (Image != null)
                {
                    categoryViewModel.Image = _categoryAccess.UploadImage(Image);
                }
                if (Icon != null)
                {
                    categoryViewModel.Icon = _categoryAccess.UploadImage(Icon);
                }
                if (categoryViewModel.CategoryKey == null || categoryViewModel.CategoryKey == Guid.Empty)
                {
                    var featureCategory = new FeatureCategory();
                    if (!string.IsNullOrEmpty(categoryViewModel.FeatureCategoryKey))
                    {
                        featureCategory = await _featureCategoryAccess.GetByKey(Guid.Parse(categoryViewModel.FeatureCategoryKey));
                        categoryViewModel.FeatureCategoryId = featureCategory.FeatureCategoryId;
                    }
                    await _categoryAccess.Insert(categoryViewModel);
                }
                else
                {
                    Category node = await _categoryAccess.GetByKey(categoryViewModel.CategoryKey);
                    categoryViewModel.FeatureCategoryId = node.FeatureCategoryId;
                    await _categoryAccess.Update(categoryViewModel);
                }
                if(categoryViewModel.FeatureCategoryKey != null)
                {
                    return RedirectToAction("Index", "Category", new { fCatagoryKey = categoryViewModel.FeatureCategoryKey});
                }
                else
                {
                    return RedirectToAction("Index", "Category");
                }

            }
            catch (Exception ex)
            {

                return RedirectToAction("Index", "Category");
            }

        }
    }
}