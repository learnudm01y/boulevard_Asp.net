using Boulevard.BaseRepository;
using Boulevard.Helper;
using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Boulevard.Service.WebAPI
{
    public class CityAccess
    {
        public IUnitOfWork uow;
        public CityAccess()
        {
            uow = new UnitOfWork();
        }

        public async Task<List<City>> GetAll(string lang="en")
            {
            try
            {
                var result =  await uow.CityRepository.Get().Where(p => p.Status.ToLower() == "active").ToListAsync();

                if (result != null && result.Count > 0)
                {
                    foreach (var res in result)
                    {
                        res.CityName = lang == "ar" ? res.CityNameAr : res.CityName;
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
        public async Task<City> GetById(int cityId,string lang="en")
        {
            try
            {
                var result =  await uow.CityRepository.GetById(cityId);
                if (result != null)
                {
                    result.CityName = lang == "ar" ? result.CityNameAr : result.CityName;
                }
                return result;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }
        public async Task<List<City>> GetCitiesByCountryId(int countryId,string lang="en")
        {
            try
            {
                var result =  await uow.CityRepository.Get().Where(p=> p.CountryId== countryId && p.Status.ToLower() == "active").ToListAsync();
                if (result != null && result.Count > 0)
                {
                    foreach (var res in result)
                    {
                        res.CityName = lang == "ar" ? res.CityNameAr : res.CityName;
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

    }
}