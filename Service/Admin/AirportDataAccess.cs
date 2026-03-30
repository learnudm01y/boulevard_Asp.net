using Boulevard.BaseRepository;
using Boulevard.Contexts;
using Boulevard.Helper;
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
    public class AirportDataAccess
    {
        public IUnitOfWork uow;

        public AirportDataAccess()
        {
            uow = new UnitOfWork();
        }
        /// <summary>
        /// Get All Airport
        /// </summary>
        /// <returns></returns>
        public async Task<List<Airport>> GetAll()
        {
            try
            {
                return await uow.AirportRepository.GetAll().Where(a => a.Status == "Active").OrderByDescending(t => t.AirportId).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Get Airport By Key
        /// </summary>
        /// <param name="airportKey"></param>
        /// <returns></returns>
        public async Task<Airport> GetAirportByKey(string airportKey)
        {
            try
            {
                return await uow.AirportRepository.GetAll(a => a.AirportKey.ToString() == airportKey).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Create Airport
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Airport> Create(Airport model)
        {
            try
            {
                model.AirportKey = Guid.NewGuid();
                model.CreateDate = DateTimeHelper.DubaiTime();
                model.CreateBy = 1;
                model.Status = "Active";
                return await uow.AirportRepository.Add(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Update Airport
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Update(Airport model)
        {
            try
            {
                model.UpdateDate = DateTimeHelper.DubaiTime();
                model.UpdateBy = 1;
                var db = new BoulevardDbContext();
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                //return await uow.MemberShipRepository.Edit(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Delete Airport
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> Delete(string key)
        {
            try
            {
                var exitResult = await GetAirportByKey(key);
                if (exitResult != null)
                {
                    exitResult.DeleteBy = 1;
                    exitResult.DeleteDate = DateTimeHelper.DubaiTime();
                    exitResult.Status = "Deleted";
                    await uow.AirportRepository.Edit(exitResult);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return false;
            }
        }
    }
}