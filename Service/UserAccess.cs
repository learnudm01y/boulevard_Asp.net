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

namespace Boulevard.Service
{
    public class UserAccess
    {
        public IUnitOfWork uow;
        public UserAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get All
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetAll()
        {
            try
            {
                return await uow.UserRepository.Get().Include(p=>p.Role).Where(e => e.Status == "Active").ToListAsync();
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
        public async Task<User> GetById(int id)
        {
            try
            {
                return await uow.UserRepository.GetById(id);
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
        public async Task<User> GetByKey(Guid key)
        {
            try
            {
                return await uow.UserRepository.Get().Where(t => t.UserKey == key).FirstOrDefaultAsync();
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
        public async Task<User> Insert(User user)
        {
            try
            {
                if (!uow.UserRepository.Get().Any(p => p.UserName.ToLower() == user.UserName.ToLower()))
                {
                    user.UserKey = Guid.NewGuid();
                    user.CreateBy = 1;
                    user.Status = "Active";
                    user.CreateDate = DateTimeHelper.DubaiTime();
                    user.Password = HashConfig.GetHash(user.Password);
                    return await uow.UserRepository.Add(user);
                }
                return null;
               
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
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<User> Update(User user)
        {
            try
            {
                if (!uow.UserRepository.Get().Any(p => p.UserId!=user.UserId && p.UserName.ToLower() == user.UserName.ToLower()))
                {
                    user.Status = "Active";
                    user.UpdateBy = 1;
                    user.UpdateDate = DateTimeHelper.DubaiTime();
                    return await uow.UserRepository.Edit(user);
                }
                return null;
                
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
                    exitResult.DeleteDate = DateTimeHelper.CreateDate();
                    await uow.UserRepository.Edit(exitResult);
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
        /// Get By auth
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public async Task<User> GetUserByAuth(string userName, string Password)
        {
            var hashPass = HashConfig.GetHash(Password);
            return uow.UserRepository.Get().Where(t => t.Status == "Active" && t.UserName == userName && t.Password == hashPass).FirstOrDefault();
        }

        /// <summary>
        /// Upload Image
        /// </summary>
        /// <param name="flagImage"></param>
        /// <returns></returns>
        public string UploadImage(HttpPostedFileBase flagImage)
        {
            string ImageName = string.Empty;
            string Url = "/Content/Upload/Users";
            ImageName = MediaHelper.UploadImage(flagImage, Url);
            return ImageName;
        }

    }
}