using Boulevard.Areas.Admin.Data;
using Boulevard.BaseRepository;
using Boulevard.Contexts;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Boulevard.Models;
using Boulevard.Helper;

namespace Boulevard.Service.Admin
{
    public class TempServiceTypeDataAccess
    {
        public IUnitOfWork uow;

        public TempServiceTypeDataAccess()
        {
            uow = new UnitOfWork();
        }

        public TempProductCountViewModel GetTempServiceTypeCount()
        {
            var db = new BoulevardDbContext();
            TempProductCountViewModel tempProductCount = new TempProductCountViewModel();

            int DoneCount = db.TempServiceTypes.Count();
            int TotalDuplicate = (from temp in db.TempServiceTypes
                                  where db.ServiceTypes.Any(f => f.ServiceTypeName == temp.ServiceTypeName)
                                  select temp).Count();

            int TotalNew = (from temp in db.TempServiceTypes
                            where !db.ServiceTypes.Any(f => f.ServiceTypeName == temp.ServiceTypeName)
                            select temp).Count();

            tempProductCount.DoneCount = DoneCount;
            if (db.TempProducts.Count() > 0)
            {
                var db1 = new BoulevardDbContext();
                tempProductCount.TotalCount = db1.TempServiceTypes.Count() > 0 ? db1.TempServiceTypes.FirstOrDefault().ExcelCount : 0;
            }
            //tempProductCount.TotalCount = db.TempProducts.Count() > 0 ? db.TempProducts.FirstOrDefault().ExcelCount : 0;
            tempProductCount.TotalNew = TotalNew;
            tempProductCount.TotalDuplicate = TotalDuplicate;

            return tempProductCount;
        }

        public void DeleteTempServicesType()
        {
            var db = new BoulevardDbContext();
            db.TempServiceTypes.RemoveRange(db.TempServiceTypes);
            db.SaveChanges();
        }

        public bool AddServiceType(int feacherCategoryId)
        {
            try
            {
                var db = new BoulevardDbContext();
                var tempServicetype = db.TempServiceTypes.Where(t => t.FeatureCategoryId == feacherCategoryId).ToList();
                foreach (var item in tempServicetype)
                {
                    var service = db.Services.Where(t=>t.Name.ToLower().Trim() == item.ServiceName.ToLower().Trim()).FirstOrDefault();
                    if(service != null)
                    {

                        var serviceType = db.ServiceTypes.FirstOrDefault(s => s.ServiceTypeName.Trim().ToLower() == item.ServiceTypeName.Trim().ToLower() && s.ServiceId == service.ServiceId);
                        if (serviceType != null)
                        {
                            // Add Service Type
                            serviceType = new ServiceType
                            {
                                ServiceTypeName = item.ServiceTypeName.Trim(),
                                ServiceTypeKey = Guid.NewGuid(),
                                CreateBy = 1,
                                CreateDate = DateTimeHelper.DubaiTime(),
                                PersoneQuantity = !string.IsNullOrEmpty(item.PersoneQuantity) ? Convert.ToInt32(item.PersoneQuantity) : 0,
                                Description = item.Description,
                                Size = item.Size,
                                Price = !string.IsNullOrEmpty(item.Price) ? Convert.ToDouble(item.Price) : 0,
                                AdultQuantity = item.AdultQuantity != null ? Convert.ToInt32(item.AdultQuantity) : 0,
                                ChildrenQuantity = item.ChildrenQuantity != null ? Convert.ToInt32(item.ChildrenQuantity) : 0,
                                ServiceHour = !string.IsNullOrEmpty(item.ServiceHour) ? Convert.ToInt32(item.ServiceHour) : 0,
                                ServiceMin = !string.IsNullOrEmpty(item.ServiceMinutes) ? Convert.ToInt32(item.ServiceMinutes) : 0,
                                ServiceId = service.ServiceId,
                                PaymentType = item.PaymentType
                            };
                            db.ServiceTypes.Add(serviceType);
                            db.SaveChanges();
                        }

                        // Add Service Type Images

                        if (!string.IsNullOrEmpty(item.Images))
                        {
                            var image = item.Images.Split(',');
                            foreach (var item1 in image)
                            {
                                if (!string.IsNullOrEmpty(item1))
                                {
                                    var serviceTypeImage = new ServiceTypeFile
                                    {
                                        ServiceTypeId = serviceType.ServiceTypeId,
                                        FileLocation = "/Content/Upload/Service/" + item1.Trim(),
                                        FileSource = "TypeImage",
                                        FileType = "Image",                                      
                                        LastUpdate = DateTimeHelper.DubaiTime(),                                       
                                    };
                                    db.ServiceTypeFiles.Add(serviceTypeImage);
                                    db.SaveChanges();
                                }
                            }
                        }


                        // Add Service Amenity

                        var serviceAmenity = db.ServiceAmenities.FirstOrDefault(s => s.AmenitiesName.Trim().ToLower() == item.AmenitiesName.Trim().ToLower() && s.ServiceId == service.ServiceId);
                        if (serviceAmenity != null)
                        {
                            serviceAmenity = new ServiceAmenity
                            {
                                ServiceAmenityKey = Guid.NewGuid(),
                                AmenitiesName = item.AmenitiesName.Trim(),
                                ServiceId = service.ServiceId,
                                CreateBy = 1,
                                CreateDate = DateTimeHelper.DubaiTime(),
                                Status = "Active",
                                AmenitiesLogo = !string.IsNullOrEmpty(item.AmenitiesLogo) ? "/Content/Upload/ServiceAmenity/" + item.AmenitiesLogo.Trim() : null
                            };
                            db.ServiceAmenities.Add(serviceAmenity);
                            db.SaveChanges();
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> AddTempServiceType(string xmlFileData, int feacherCategoryId)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["BoulevardDbContext"].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@tempServiceTypes_xml", xmlFileData, DbType.Xml);
                    parameters.Add("@feacherCategoryId", feacherCategoryId);
                    await connection.ExecuteAsync("pr_upload_bulk_service_Type", parameters, commandType: CommandType.StoredProcedure);

                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}