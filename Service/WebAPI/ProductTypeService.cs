using Boulevard.BaseRepository;
using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Boulevard.Service.WebAPI
{
    public class ProductTypeService
    {
        public IUnitOfWork _uow;
        public ProductTypeService()
        {
            _uow = new UnitOfWork();
        }

        // Get All Product Types
        public async Task<List<ProductTypeMaster>> GetAllProductTypesAsync(string lang = "en")
        {
            try
            {
                var data = await _uow.ProductTypeMasterRepository
                .Get().Where(x => x.Status == "Active")
                .OrderBy(x => x.ProductTypeId)
                .ToListAsync();

                if (data != null && data.Count() > 0)
                {
                    foreach (var dd in data)
                    {
                        dd.Name = lang=="en"?dd.Name:dd.NameAr;
                        dd.Description = lang == "en" ? dd.Description : dd.DescriptionAr;
                        dd.DeliveryTime = lang == "en" ? dd.DeliveryTime : dd.DeliveryTime;
                    }
                }
                return data;

            }
            catch (Exception)
            {

                throw;
            }

           
        }
    }
}