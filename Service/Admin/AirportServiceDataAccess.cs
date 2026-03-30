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
    public class AirportServiceDataAccess
    {
        public IUnitOfWork uow;

        public AirportServiceDataAccess()
        {
            uow = new UnitOfWork();
        }

        public async Task<List<AirportService>> GetByServiceId(int ServiceId)
        {
            try
            {
                return await uow.AirportServiceRepository.Get().Where(a => a.ServiceId== ServiceId && a.Status == "Active").ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
        public async Task RemoveByServiceId(int ServiceId)
        {
            try
            {
                List<AirportService> aser = await uow.AirportServiceRepository.Get().Where(p => p.ServiceId == ServiceId).ToListAsync();          
                uow.AirportServiceRepository.MultipleRemove(aser);

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
        public async Task InsertByServiceId(List<Airport> Airports, int ServiceId)
        {
            try
            {
                foreach (var node in Airports)
                {
                    if (node.IsSelected)
                    {
                        AirportService obj = new AirportService();
                        obj.AirportId = node.AirportId;
                        obj.ServiceId = ServiceId;
                        obj.CreateBy = 1;
                        obj.Status = "Active";
                        obj.CreateDate = DateTimeHelper.DubaiTime();
                        await uow.AirportServiceRepository.Add(obj);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

    }
}