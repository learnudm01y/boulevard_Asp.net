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
    public class DeliverySettingsServiceAccess
    {
        public IUnitOfWork uow;
        public DeliverySettingsServiceAccess()
        {
            uow = new UnitOfWork();
        }

        public async Task<DeliverySetting> GetDeliveryInfo(int featureCategoryId)
        {
            try
            {
                return await uow.DeliverySettingRepository.Get().Where(p => p.Status.ToLower() == "active" && p.FeatureCategoryId== featureCategoryId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }
    }
}