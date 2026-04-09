using Boulevard.Contexts;
using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Boulevard.Areas.Admin.Controllers
{
    public class ServiceFormConfigController : BaseController
    {
        // Typing FeatureCategoryKey
        private static readonly Guid TypingCategoryKey = new Guid("f4309df5-9121-41ad-831a-994c46b62766");

        // ──────────────────────────────────────────────
        //  INDEX – list all ServiceTypes inside Typing category with pagination
        // ──────────────────────────────────────────────
        public async Task<ActionResult> Index(string search = "", int page = 1)
        {
            const int pageSize = 20;
            using (var db = new BoulevardDbContext())
            {
                var fc = await db.featureCategories
                    .FirstOrDefaultAsync(f => f.FeatureCategoryKey == TypingCategoryKey && !f.IsDelete);

                int typingCatId = fc?.FeatureCategoryId ?? 0;

                var query = db.ServiceTypes
                    .Include("Service")
                    .Where(st => st.Service.FeatureCategoryId == typingCatId)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                    query = query.Where(st => st.ServiceTypeName.Contains(search) || st.ServiceTypeNameAr.Contains(search));

                int totalCount = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
                if (page < 1) page = 1;
                if (page > totalPages && totalPages > 0) page = totalPages;

                var items = await query
                    .OrderBy(st => st.ServiceTypeName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Count configured sections per service type
                var stIds = items.Select(st => st.ServiceTypeId).ToList();
                var sectionCounts = await db.ServiceFormSections
                    .Where(sec => stIds.Contains(sec.ServiceTypeId) && !sec.IsDelete)
                    .GroupBy(sec => sec.ServiceTypeId)
                    .Select(g => new { ServiceTypeId = g.Key, Count = g.Count() })
                    .ToListAsync();

                var countDict = sectionCounts.ToDictionary(x => x.ServiceTypeId, x => x.Count);

                ViewBag.ServiceTypes = items;
                ViewBag.SectionCounts = countDict;
                ViewBag.Search = search;
                ViewBag.Page = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.TotalCount = totalCount;
                ViewBag.TypingCategoryName = fc?.Name ?? "Typing Services";
                return View();
            }
        }

        // ──────────────────────────────────────────────
        //  CONFIG – show sections & fields for a specific service type
        // ──────────────────────────────────────────────
        public async Task<ActionResult> Config(int serviceTypeId)
        {
            using (var db = new BoulevardDbContext())
            {
                var st = await db.ServiceTypes
                    .Include("Service")
                    .FirstOrDefaultAsync(s => s.ServiceTypeId == serviceTypeId);
                if (st == null)
                    return RedirectToAction("Index");

                ViewBag.ServiceTypeId = serviceTypeId;
                ViewBag.ServiceName = st.ServiceTypeName;
                ViewBag.ServiceNameAr = st.ServiceTypeNameAr;
                ViewBag.ParentServiceName = st.Service?.Name;

                var sections = await db.ServiceFormSections
                    .Include("Fields")
                    .Include("Fields.Options")
                    .Include("Fields.AttachmentRules")
                    .Where(s => s.ServiceTypeId == serviceTypeId && !s.IsDelete)
                    .OrderBy(s => s.SortOrder)
                    .ToListAsync();

                foreach (var sec in sections)
                {
                    sec.Fields = sec.Fields
                        .Where(f => !f.IsDelete)
                        .OrderBy(f => f.SortOrder)
                        .ToList();

                    foreach (var fld in sec.Fields)
                    {
                        fld.Options = fld.Options
                            .Where(o => !o.IsDelete)
                            .OrderBy(o => o.SortOrder)
                            .ToList();
                    }
                }

                ViewBag.Sections = sections;
                return View();
            }
        }

        // ──────────────────────────────────────────────
        //  SECTION CRUD (AJAX)
        // ──────────────────────────────────────────────
        [HttpPost]
        public async Task<ActionResult> AddSection(int serviceTypeId, string title, string titleAr)
        {
            using (var db = new BoulevardDbContext())
            {
                int maxOrder = await db.ServiceFormSections
                    .Where(s => s.ServiceTypeId == serviceTypeId && !s.IsDelete)
                    .Select(s => (int?)s.SortOrder)
                    .MaxAsync() ?? 0;

                var section = new ServiceFormSection
                {
                    ServiceTypeId = serviceTypeId,
                    Title = title,
                    TitleAr = titleAr,
                    SortOrder = maxOrder + 1,
                    IsActive = true,
                    IsDelete = false,
                    CreateBy = GetUser()?.UserId ?? 1,
                    CreateDate = DateTime.Now
                };

                db.ServiceFormSections.Add(section);
                await db.SaveChangesAsync();

                return Json(new { success = true, sectionId = section.SectionId });
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateSection(int sectionId, string title, string titleAr, bool isActive)
        {
            using (var db = new BoulevardDbContext())
            {
                var section = await db.ServiceFormSections.FindAsync(sectionId);
                if (section == null)
                    return Json(new { success = false, message = "Section not found." });

                section.Title = title;
                section.TitleAr = titleAr;
                section.IsActive = isActive;
                section.UpdateBy = GetUser()?.UserId ?? 1;
                section.UpdateDate = DateTime.Now;

                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteSection(int sectionId)
        {
            using (var db = new BoulevardDbContext())
            {
                var section = await db.ServiceFormSections.FindAsync(sectionId);
                if (section == null)
                    return Json(new { success = false, message = "Section not found." });

                section.IsDelete = true;
                section.UpdateDate = DateTime.Now;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
        }

        [HttpPost]
        public async Task<ActionResult> ReorderSections(List<int> sectionIds)
        {
            using (var db = new BoulevardDbContext())
            {
                for (int i = 0; i < sectionIds.Count; i++)
                {
                    var section = await db.ServiceFormSections.FindAsync(sectionIds[i]);
                    if (section != null)
                        section.SortOrder = i + 1;
                }
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
        }

        // ──────────────────────────────────────────────
        //  FIELD CRUD (AJAX)
        // ──────────────────────────────────────────────
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> SaveField(
            int fieldId,
            int sectionId,
            string fieldKey,
            string label,
            string labelAr,
            string placeholder,
            string placeholderAr,
            string fieldType,
            string dataType,
            bool isRequired,
            bool isVisible,
            string defaultValue,
            string helpText,
            string helpTextAr,
            string validationRegex,
            int? minLength,
            int? maxLength,
            string minValue,
            string maxValue)
        {
            using (var db = new BoulevardDbContext())
            {
                ServiceFormField field;

                if (fieldId > 0)
                {
                    field = await db.ServiceFormFields.FindAsync(fieldId);
                    if (field == null)
                        return Json(new { success = false, message = "Field not found." });
                }
                else
                {
                    int maxOrder = await db.ServiceFormFields
                        .Where(f => f.SectionId == sectionId && !f.IsDelete)
                        .Select(f => (int?)f.SortOrder)
                        .MaxAsync() ?? 0;

                    field = new ServiceFormField
                    {
                        SectionId = sectionId,
                        SortOrder = maxOrder + 1,
                        IsDelete = false,
                        CreateBy = GetUser()?.UserId ?? 1,
                        CreateDate = DateTime.Now
                    };
                    db.ServiceFormFields.Add(field);
                }

                field.FieldKey = fieldKey;
                field.Label = label;
                field.LabelAr = labelAr;
                field.Placeholder = placeholder;
                field.PlaceholderAr = placeholderAr;
                field.FieldType = fieldType;
                field.DataType = dataType;
                field.IsRequired = isRequired;
                field.IsVisible = isVisible;
                field.DefaultValue = defaultValue;
                field.HelpText = helpText;
                field.HelpTextAr = helpTextAr;
                field.ValidationRegex = validationRegex;
                field.MinLength = minLength;
                field.MaxLength = maxLength;
                field.MinValue = minValue;
                field.MaxValue = maxValue;
                field.UpdateBy = GetUser()?.UserId ?? 1;
                field.UpdateDate = DateTime.Now;

                await db.SaveChangesAsync();
                return Json(new { success = true, fieldId = field.FieldId });
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteField(int fieldId)
        {
            using (var db = new BoulevardDbContext())
            {
                var field = await db.ServiceFormFields.FindAsync(fieldId);
                if (field == null)
                    return Json(new { success = false, message = "Field not found." });

                field.IsDelete = true;
                field.UpdateDate = DateTime.Now;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
        }

        [HttpPost]
        public async Task<ActionResult> ReorderFields(List<int> fieldIds)
        {
            using (var db = new BoulevardDbContext())
            {
                for (int i = 0; i < fieldIds.Count; i++)
                {
                    var field = await db.ServiceFormFields.FindAsync(fieldIds[i]);
                    if (field != null)
                        field.SortOrder = i + 1;
                }
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
        }

        // ──────────────────────────────────────────────
        //  OPTIONS CRUD (AJAX)
        // ──────────────────────────────────────────────
        [HttpPost]
        public async Task<ActionResult> SaveOption(int optionId, int fieldId, string optionLabel, string optionLabelAr, string optionValue, bool isDefault)
        {
            using (var db = new BoulevardDbContext())
            {
                ServiceFormFieldOption opt;

                if (optionId > 0)
                {
                    opt = await db.ServiceFormFieldOptions.FindAsync(optionId);
                    if (opt == null)
                        return Json(new { success = false, message = "Option not found." });
                }
                else
                {
                    int maxOrder = await db.ServiceFormFieldOptions
                        .Where(o => o.FieldId == fieldId && !o.IsDelete)
                        .Select(o => (int?)o.SortOrder)
                        .MaxAsync() ?? 0;

                    opt = new ServiceFormFieldOption
                    {
                        FieldId = fieldId,
                        SortOrder = maxOrder + 1,
                        IsDelete = false
                    };
                    db.ServiceFormFieldOptions.Add(opt);
                }

                opt.OptionLabel = optionLabel;
                opt.OptionLabelAr = optionLabelAr;
                opt.OptionValue = optionValue;
                opt.IsDefault = isDefault;

                await db.SaveChangesAsync();
                return Json(new { success = true, optionId = opt.OptionId });
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteOption(int optionId)
        {
            using (var db = new BoulevardDbContext())
            {
                var opt = await db.ServiceFormFieldOptions.FindAsync(optionId);
                if (opt == null)
                    return Json(new { success = false, message = "Option not found." });

                opt.IsDelete = true;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
        }

        // ──────────────────────────────────────────────
        //  ATTACHMENT RULE CRUD (AJAX)
        // ──────────────────────────────────────────────
        [HttpPost]
        public async Task<ActionResult> SaveAttachmentRule(
            int attachmentRuleId,
            int fieldId,
            string allowedExtensions,
            int maxFileSizeMB,
            bool allowMultiple,
            bool isRequired,
            int maxFileCount,
            string displayLabel,
            string displayLabelAr)
        {
            using (var db = new BoulevardDbContext())
            {
                ServiceFormAttachmentRule rule;

                if (attachmentRuleId > 0)
                {
                    rule = await db.ServiceFormAttachmentRules.FindAsync(attachmentRuleId);
                    if (rule == null)
                        return Json(new { success = false, message = "Attachment rule not found." });
                }
                else
                {
                    rule = new ServiceFormAttachmentRule { FieldId = fieldId };
                    db.ServiceFormAttachmentRules.Add(rule);
                }

                rule.AllowedExtensions = allowedExtensions;
                rule.MaxFileSizeMB = maxFileSizeMB;
                rule.AllowMultiple = allowMultiple;
                rule.IsRequired = isRequired;
                rule.MaxFileCount = maxFileCount;
                rule.DisplayLabel = displayLabel;
                rule.DisplayLabelAr = displayLabelAr;

                await db.SaveChangesAsync();
                return Json(new { success = true, attachmentRuleId = rule.AttachmentRuleId });
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAttachmentRule(int attachmentRuleId)
        {
            using (var db = new BoulevardDbContext())
            {
                var rule = await db.ServiceFormAttachmentRules.FindAsync(attachmentRuleId);
                if (rule == null)
                    return Json(new { success = false, message = "Attachment rule not found." });

                db.ServiceFormAttachmentRules.Remove(rule);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
        }

        // ──────────────────────────────────────────────
        //  GET field data for edit modal (AJAX)
        // ──────────────────────────────────────────────
        [HttpGet]
        public async Task<ActionResult> GetField(int fieldId)
        {
            using (var db = new BoulevardDbContext())
            {
                var f = await db.ServiceFormFields
                    .Include("Options")
                    .Include("AttachmentRules")
                    .FirstOrDefaultAsync(x => x.FieldId == fieldId);

                if (f == null)
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);

                var options = f.Options?.Where(o => !o.IsDelete).OrderBy(o => o.SortOrder).Select(o => new
                {
                    o.OptionId,
                    o.OptionLabel,
                    o.OptionLabelAr,
                    o.OptionValue,
                    o.SortOrder,
                    o.IsDefault
                }).ToList();

                object attRule = null;
                var attachmentRule = f.AttachmentRules?.FirstOrDefault();
                if (attachmentRule != null)
                {
                    var a = attachmentRule;
                    attRule = new
                    {
                        a.AttachmentRuleId,
                        a.AllowedExtensions,
                        a.MaxFileSizeMB,
                        a.AllowMultiple,
                        a.IsRequired,
                        a.MaxFileCount,
                        a.DisplayLabel,
                        a.DisplayLabelAr
                    };
                }

                return Json(new
                {
                    success = true,
                    field = new
                    {
                        f.FieldId,
                        f.SectionId,
                        f.FieldKey,
                        f.Label,
                        f.LabelAr,
                        f.Placeholder,
                        f.PlaceholderAr,
                        f.FieldType,
                        f.DataType,
                        f.IsRequired,
                        f.IsVisible,
                        f.DefaultValue,
                        f.HelpText,
                        f.HelpTextAr,
                        f.ValidationRegex,
                        f.MinLength,
                        f.MaxLength,
                        f.MinValue,
                        f.MaxValue
                    },
                    options,
                    attachmentRule = attRule
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
