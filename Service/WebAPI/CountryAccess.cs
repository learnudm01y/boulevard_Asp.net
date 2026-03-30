using Boulevard.BaseRepository;
using Boulevard.Helper;
using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;

namespace Boulevard.Service.WebAPI
{
    public class CountryAccess
    {
        public IUnitOfWork uow;
        public CountryAccess()
        {
            uow = new UnitOfWork();
        }

        public async Task<List<Country>> GetAll(string lang = "en")
            {
            try
            {
                var result =  await uow.CountryRepository.Get().Where(p => p.Status.ToLower() == "active").ToListAsync();
                if (result != null && result.Count > 0)
                {
                    foreach (var res in result)
                    {
                        res.CountryName = lang == "ar" ? res.CountryNameAr : res.CountryName;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }
        public async Task<Country> GetById(int countryId, string lang = "en")
        {
            try
            {
                var result =  await uow.CountryRepository.GetById(countryId);
                if (result != null )
                {

                    result.CountryName = lang == "ar" ? result.CountryNameAr : result.CountryName;
                    
                }
                return result;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }
        
    }
}