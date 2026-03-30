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
    public class PaymentMethodServiceAccess
    {
        public IUnitOfWork uow;
        public PaymentMethodServiceAccess()
        {
            uow = new UnitOfWork();
        }

        public async Task<List<PaymentMethod>> getallpayment(string lang="en")
        {
            try
            {
                var result = await uow.PaymentMethodRepository.Get().Where(s=>s.Status=="Active").ToListAsync();
                if (result != null && result.Count > 0)
                {
                    foreach (var res in result)
                    {
                        res.PaymentMethodName = lang == "ar" ? res.PaymentMethodNameAr : res.PaymentMethodName;
                    }
                }
                return result;

            }
            catch (Exception)
            {

                return null;
            }
        }
    }
}