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
    public class AirportAccess
    {
        public IUnitOfWork uow;
        public AirportAccess()
        {
            uow = new UnitOfWork();
        }

        public async Task<List<Airport>> GetAll()
        {
            try
            {
                return await uow.AirportRepository.Get().Include(a => a.City).Where(p => p.Status == "Active").ToListAsync();
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }
        public async Task<Airport> GetById(int AirportId)
        {
            try
            {
                return await uow.AirportRepository.Get().Include(a => a.City).Where(p => p.AirportId == AirportId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }


    }
}