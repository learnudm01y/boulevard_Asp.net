using Boulevard.BaseRepository;
using Boulevard.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Boulevard.Service.Admin
{
    public class CustomerEnqueryDataAccess
    {
        public IUnitOfWork uow;

        public CustomerEnqueryDataAccess()
        {
            uow = new UnitOfWork();
        }
        /// <summary>
        /// Get All Customer Enquery
        /// </summary>
        /// <returns></returns>
        public async Task<List<CustomerEnquery>> GetAll()
        {
            try
            {
                var dataModel = await uow.CustomerEnqueryRepository.GetAll().Where(a => a.Status == "Active").OrderByDescending(t => t.CustomerEnqueryId).ToListAsync();
                foreach(var item in dataModel)
                {
                    var fCategoryName = await uow.FeatureCategoryRepository.Get().Where(a => a.FeatureCategoryId == item.FeatureCategoryId).Select(a => a.Name).FirstOrDefaultAsync();
                    if (fCategoryName != null)
                    {
                        item.FeatureCategoryName = fCategoryName;
                    }
                    var userName = await uow.MemberRepository.Get().Where(a => a.MemberId == item.UserId).Select(a => a.Name).FirstOrDefaultAsync();
                    if (userName != null)
                    {
                        item.UserName = userName;
                    }
                }
                return dataModel;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

       
    }
}