using Boulevard.Contexts;
using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Boulevard.Controllers
{
    /// <summary>
    /// Service Form Configuration API — returns the full dynamic field schema
    /// for any individual service (e.g. a specific Typing service) so Flutter
    /// can render forms from JSON.
    /// Base URL: /api/v1/service-form
    /// </summary>
    public class ServiceFormController : BaseController
    {
        /// <summary>
        /// GET /api/v1/service-form/schema/{serviceKey}
        /// Returns the complete form schema (sections → fields → options + attachment rules)
        /// for the given service identified by its ServiceKey (GUID from Services table).
        /// </summary>
        [HttpGet]
        public async Task<IHttpActionResult> GetSchema(Guid serviceTypeKey, string lang = "en")
        {
            try
            {
                using (var db = new BoulevardDbContext())
                {
                    var serviceType = await db.ServiceTypes
                        .Include("Service")
                        .FirstOrDefaultAsync(st => st.ServiceTypeKey == serviceTypeKey);

                    if (serviceType == null)
                        return ErrorMessage("Service type not found.");

                    var sections = await db.ServiceFormSections
                        .Where(s => s.ServiceTypeId == serviceType.ServiceTypeId && !s.IsDelete && s.IsActive)
                        .OrderBy(s => s.SortOrder)
                        .ToListAsync();

                    var sectionIds = sections.Select(s => s.SectionId).ToList();

                    var fields = await db.ServiceFormFields
                        .Where(f => sectionIds.Contains(f.SectionId) && !f.IsDelete && f.IsVisible)
                        .OrderBy(f => f.SortOrder)
                        .ToListAsync();

                    var fieldIds = fields.Select(f => f.FieldId).ToList();

                    var options = await db.ServiceFormFieldOptions
                        .Where(o => fieldIds.Contains(o.FieldId) && !o.IsDelete)
                        .OrderBy(o => o.SortOrder)
                        .ToListAsync();

                    var attachmentRules = await db.ServiceFormAttachmentRules
                        .Where(a => fieldIds.Contains(a.FieldId))
                        .ToListAsync();

                    bool isAr = lang == "ar";

                    var result = new
                    {
                        serviceTypeTitle = isAr ? (serviceType.ServiceTypeNameAr ?? serviceType.ServiceTypeName) : serviceType.ServiceTypeName,
                        serviceTypeTitleAr = serviceType.ServiceTypeNameAr,
                        serviceTypeKey = serviceType.ServiceTypeKey,
                        serviceTypeId = serviceType.ServiceTypeId,
                        parentServiceName = serviceType.Service?.Name,
                        sections = sections.Select(s => new
                        {
                            sectionId = s.SectionId,
                            title = isAr ? (s.TitleAr ?? s.Title) : s.Title,
                            titleAr = s.TitleAr,
                            sortOrder = s.SortOrder,
                            fields = fields
                                .Where(f => f.SectionId == s.SectionId)
                                .Select(f =>
                                {
                                    var fieldOpts = options.Where(o => o.FieldId == f.FieldId).ToList();
                                    var attRule = attachmentRules.FirstOrDefault(a => a.FieldId == f.FieldId);

                                    var fieldObj = new Dictionary<string, object>
                                    {
                                        ["fieldId"] = f.FieldId,
                                        ["fieldKey"] = f.FieldKey,
                                        ["label"] = isAr ? (f.LabelAr ?? f.Label) : f.Label,
                                        ["labelAr"] = f.LabelAr,
                                        ["placeholder"] = isAr ? (f.PlaceholderAr ?? f.Placeholder) : f.Placeholder,
                                        ["fieldType"] = f.FieldType,
                                        ["dataType"] = f.DataType,
                                        ["isRequired"] = f.IsRequired,
                                        ["sortOrder"] = f.SortOrder,
                                        ["defaultValue"] = f.DefaultValue,
                                        ["helpText"] = isAr ? (f.HelpTextAr ?? f.HelpText) : f.HelpText,
                                        ["validation"] = new
                                        {
                                            regex = f.ValidationRegex,
                                            minLength = f.MinLength,
                                            maxLength = f.MaxLength,
                                            minValue = f.MinValue,
                                            maxValue = f.MaxValue
                                        }
                                    };

                                    if (fieldOpts.Any())
                                    {
                                        fieldObj["options"] = fieldOpts.Select(o => new
                                        {
                                            value = o.OptionValue,
                                            label = isAr ? (o.OptionLabelAr ?? o.OptionLabel) : o.OptionLabel,
                                            labelAr = o.OptionLabelAr,
                                            isDefault = o.IsDefault
                                        }).ToList();
                                    }

                                    if (attRule != null)
                                    {
                                        fieldObj["attachment"] = new
                                        {
                                            allowedExtensions = attRule.AllowedExtensions
                                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(e => e.Trim()).ToList(),
                                            maxFileSizeMB = attRule.MaxFileSizeMB,
                                            allowMultiple = attRule.AllowMultiple,
                                            isRequired = attRule.IsRequired,
                                            maxFileCount = attRule.MaxFileCount,
                                            displayLabel = isAr
                                                ? (attRule.DisplayLabelAr ?? attRule.DisplayLabel)
                                                : attRule.DisplayLabel
                                        };
                                    }

                                    return fieldObj;
                                }).ToList()
                        }).ToList()
                    };

                    return SuccessMessage(result);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "ServiceForm.GetSchema failed");
                return Content(HttpStatusCode.InternalServerError, new
                {
                    isSuccess = false,
                    message = "Internal server error",
                    code = 500,
                    result = new List<object>()
                });
            }
        }

        /// <summary>
        /// GET /api/v1/service-form/list
        /// Returns a lightweight list of all individual services that have form configurations.
        /// </summary>
        [HttpGet]
        public async Task<IHttpActionResult> GetConfiguredServices()
        {
            try
            {
                using (var db = new BoulevardDbContext())
                {
                    var configuredServiceTypeIds = await db.ServiceFormSections
                        .Where(s => !s.IsDelete && s.IsActive)
                        .Select(s => s.ServiceTypeId)
                        .Distinct()
                        .ToListAsync();

                    var serviceTypes = await db.ServiceTypes
                        .Include("Service")
                        .Where(st => configuredServiceTypeIds.Contains(st.ServiceTypeId))
                        .OrderBy(st => st.ServiceTypeName)
                        .ToListAsync();

                    // section counts per service type
                    var sectionCounts = await db.ServiceFormSections
                        .Where(s => configuredServiceTypeIds.Contains(s.ServiceTypeId) && !s.IsDelete && s.IsActive)
                        .GroupBy(s => s.ServiceTypeId)
                        .Select(g => new { ServiceTypeId = g.Key, Count = g.Count() })
                        .ToListAsync();
                    var countDict = sectionCounts.ToDictionary(x => x.ServiceTypeId, x => x.Count);

                    var result = serviceTypes.Select(st => new
                    {
                        serviceTypeKey = st.ServiceTypeKey,
                        serviceTypeId = st.ServiceTypeId,
                        name = st.ServiceTypeName,
                        nameAr = st.ServiceTypeNameAr,
                        parentServiceName = st.Service?.Name,
                        sectionCount = countDict.ContainsKey(st.ServiceTypeId) ? countDict[st.ServiceTypeId] : 0
                    }).ToList();

                    return SuccessMessage(result);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "ServiceForm.GetConfiguredServices failed");
                return Content(HttpStatusCode.InternalServerError, new
                {
                    isSuccess = false,
                    message = "Internal server error",
                    code = 500,
                    result = new List<object>()
                });
            }
        }
    }
}
