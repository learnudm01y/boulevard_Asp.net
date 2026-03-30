using Boulevard.Areas.Admin.Data;
using Boulevard.BaseRepository;
using Boulevard.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Boulevard.Service
{
    public class UserActivityAccess
    {
        public IUnitOfWork uow;
        public UserActivityAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// User Activity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void Insert(UserActivity model)
        {
            try
            {
                uow.UserActivityRepository.Addd(model);
            }
            catch (Exception ex)
            {
               Log.Error(ex.ToString());
            }
        }

        public async Task<List<UserActivityTimeLine>> GetUserActivityTimeLineByUserId(int userId)
        {
            try
            {
                List<UserActivityTimeLine> lines = new List<UserActivityTimeLine>();
                var res = await uow.UserActivityRepository.Get().Where(t => t.UserId == userId).OrderByDescending(m => m.CreateDate).ToListAsync();

                foreach (var item in res)
                {
                    item.Time = item.CreateDate.ToString("hh:mm tt");
                    if (lines.Where(t => t.Date.Date == item.CreateDate.Date).Count() == 0)
                    {
                        UserActivityTimeLine userActivityTimeLine = new UserActivityTimeLine();
                        userActivityTimeLine.Date = item.CreateDate;
                        userActivityTimeLine.UserActivities.Add(item);
                        lines.Add(userActivityTimeLine);
                    }
                    else
                    {
                        foreach (var data in lines.Where(t => t.Date.Date == item.CreateDate.Date))
                        {
                            data.UserActivities.Add(item);
                        }
                    }
                }
                return lines;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return new List<UserActivityTimeLine>();
            }
        }
    }
}