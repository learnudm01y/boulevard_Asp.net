using Boulevard.BaseRepository;
using Boulevard.Contexts;
using Boulevard.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace Boulevard.Service
{
    public class LayoutSettingAccess
    {
        public IUnitOfWork uow;
        public LayoutSettingAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Default Layout
        /// </summary>
        /// <returns></returns>
        private const string LayoutCacheKey = "LayoutSetting_Default";

        public LayoutSetting GetDefaultLayout()
        {
            try
            {
                var cached = HttpRuntime.Cache[LayoutCacheKey] as LayoutSetting;
                if (cached != null) return cached;

                var model = new LayoutSetting();
                var result = uow.LayoutSettingRepository.Get().Where(e => e.IsDefault).FirstOrDefault();
                if (result != null)
                {
                    model.Name = result.Name;
                    model.LogoHeader = result.LogoHeader;
                    model.MainHeader = result.MainHeader;
                    model.Body = result.Body;
                    model.SideBar = result.SideBar;
                }
                else
                {
                    model.Name = "Light";
                    model.LogoHeader = "skin6";
                    model.MainHeader = "skin6";
                    model.Body = "Light";
                    model.SideBar = "skin6";
                }

                HttpRuntime.Cache.Insert(LayoutCacheKey, model, null,
                    DateTime.Now.AddMinutes(10), Cache.NoSlidingExpiration);
                return model;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Layout Update
        /// </summary>
        public void layoutUpdate()
        {
            var layout = uow.LayoutSettingRepository.Get();
            foreach (var item in layout)
            {
                item.IsDefault = item.IsDefault == true ? false : true;
                var bd = new BoulevardDbContext();
                bd.Entry(item).State = EntityState.Modified;
                bd.SaveChanges();
            }
        }
    }
}