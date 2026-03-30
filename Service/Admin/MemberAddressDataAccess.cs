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
    public class MemberAddressDataAccess
    {
        public IUnitOfWork uow;

        public MemberAddressDataAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get All from Member Address
        /// </summary>
        /// <returns></returns>
        public async Task<List<MemberAddress>> GetAll()
        {
            try
            {
                return await uow.MemberAddressRepository.GetAll().Where(a => a.Status == "Active").OrderByDescending(t => t.MemberAddressId).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get Member Address By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MemberAddress> GetMemberAddressById(long id)
        {
            try
            {
                return await uow.MemberAddressRepository.GetAll(a => a.MemberAddressId == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
        
        public async Task<MemberAddress> GetMemberAddressByMemberId(long id)
        {
            try
            {
                return await uow.MemberAddressRepository.GetAll(a => a.MemberId == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get Member Address By Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<MemberAddress> GetMemberAddressByKey(string key)
        {
            try
            {
                return await uow.MemberAddressRepository.GetAll(a => a.MemberAddressKey.ToString() == key).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get Member Address By Member Id
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public async Task<List<MemberAddress>> GetMemberAddressListByMemberId(long memberId)
        {
            try
            {
                var data = await uow.MemberAddressRepository.GetAll(a => a.Status == "Active" && a.MemberId == memberId).ToListAsync();
                foreach(var item in data)
                {
                    item.Country = await uow.CountryRepository.GetById(item.CountryId);
                    item.City = await uow.CityRepository.GetById(item.CityId);
                }
                return data;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Create Member Address
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<MemberAddress> Create(MemberAddress model)
        {
            try
            {
                model.MemberAddressKey = Guid.NewGuid();
                return await uow.MemberAddressRepository.Add(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Update Member Address
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<MemberAddress> Update(MemberAddress model)
        {
            try
            {
                return await uow.MemberAddressRepository.Edit(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
    }
}