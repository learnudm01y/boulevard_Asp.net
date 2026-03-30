using Boulevard.BaseRepository;
using Boulevard.Contexts;
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
    public class CityDataAccess
    {
        public IUnitOfWork uow;

        public CityDataAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get All from City
        /// </summary>
        /// <returns></returns>
        public async Task<List<City>> GetAll()
        {
            try
            {
                return await uow.CityRepository.GetAll().Where(a => a.Status == "Active").OrderByDescending(t => t.CityId).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get All from City
        /// </summary>
        /// <returns></returns>
        public async Task<List<City>> GetCitiesByCountryId(int countryId)
        {
            try
            {
                return await uow.CityRepository.GetAll().Where(a => a.CountryId == countryId).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }


        /// <summary>
        /// Get City By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<City> GetCityById(int id)
        {
            try
            {
                return await uow.CityRepository.GetAll(a => a.CityId == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get City By Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<City> GetCityByKey(string key)
        {
            try
            {
                return await uow.CityRepository.GetAll(a => a.CityKey.ToString() == key).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Create City
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<City> Create(City model)
        {
            try
            {
                model.CityKey = Guid.NewGuid();
                return await uow.CityRepository.Add(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Update City
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<City> Update(City model)
        {
            try
            {
                return await uow.CityRepository.Edit(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
    }
}