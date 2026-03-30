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
using System.Xml.Linq;

namespace Boulevard.Service
{
    public class RoleAccess
    {
        public IUnitOfWork uow;
        public RoleAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get All
        /// </summary>
        /// <returns></returns>
        public async Task<List<Role>> GetAll()
        {
            try
            {
                return await uow.RoleRepository.Get().Where(e => e.Status == "Active").ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Get By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Role> GetById(int id)
        {
            try
            {
                return await uow.RoleRepository.GetById(id);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Get By key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<Role> GetByKey(Guid key)
        {
            try
            {
                return await uow.RoleRepository.Get().Where(t => t.RoleKey == key).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<Role> Insert(Role node)
        {
            try
            {
                node.RoleKey = Guid.NewGuid();
                node.Status = "Active";
                node.CreateBy = 1;
                node.CreateDate = DateTimeHelper.DubaiTime();
                return await uow.RoleRepository.Add(node);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<Role> Update(Role node)
        {
            try
            {
                node.UpdateBy = 1;
                node.Status = "Active";
                node.UpdateDate = DateTimeHelper.DubaiTime();
                return await uow.RoleRepository.Edit(node);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateby"></param>
        /// <returns></returns>
        public async Task<bool> Delete(Guid key, int updateby)
        {

            try
            {
                var exitResult = await GetByKey(key);
                if (exitResult != null)
                {
                    exitResult.Status = "Deleted";
                    exitResult.DeleteBy = updateby;
                    exitResult.DeleteDate = DateTimeHelper.DubaiTime();
                    await uow.RoleRepository.Edit(exitResult);
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

        

        /// <summary>
        /// Upload Image
        /// </summary>
        /// <param name="flagImage"></param>
        /// <returns></returns>
        public string UploadImage(HttpPostedFileBase flagImage)
        {
            string ImageName = string.Empty;
            string Url = "/Content/Upload/Roles";
            ImageName = MediaHelper.UploadImage(flagImage, Url);
            return ImageName;
        }

    }
}