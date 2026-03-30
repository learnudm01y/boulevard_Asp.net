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
    public class CountryDataAccess
    {
        public IUnitOfWork uow;

        public CountryDataAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get All from Country
        /// </summary>
        /// <returns></returns>
        public async Task<List<Country>> GetAll()
        {
            try
            {
                return await uow.CountryRepository.GetAll().Where(a => a.Status == "Active").OrderByDescending(t => t.CountryId).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get Country By Id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public async Task<Country> GetCountryById(int countryId)
        {
            try
            {
                return await uow.CountryRepository.GetAll(a => a.CountryId == countryId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get Country By Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<Country> GetCountryByKey(string key)
        {
            try
            {
                return await uow.CountryRepository.GetAll(a => a.CountryKey.ToString() == key).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Create Country
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Country> Create(Country model)
        {
            try
            {
                model.CountryKey = Guid.NewGuid();
                return await uow.CountryRepository.Add(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Update Country
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Country> Update(Country model)
        {
            try
            {
                return await uow.CountryRepository.Edit(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
    }
}