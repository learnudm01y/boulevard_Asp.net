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
    public class MemberAddressAccess
    {
        public IUnitOfWork uow;
        public MemberAddressAccess()
        {
            uow = new UnitOfWork();
        }

        public async Task<List<MemberAddress>> GetMemberAddress(int memberId)
            {
            try
            {
                return await uow.MemberAddressRepository.Get().Where(p => p.MemberId == memberId && p.Status.ToLower() == "active").ToListAsync();
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }
        public async Task<MemberAddress> Insert(MemberAddress model)
        {
            try
            {
                List<MemberAddress> list = await uow.MemberAddressRepository.Get().Where(p => p.MemberId == model.MemberId && p.Status.ToLower() == "active").ToListAsync();
                foreach (MemberAddress node in list)
                {
                    node.IsDefault = false;
                    await uow.MemberAddressRepository.Edit(node);
                }
                model.IsDefault = true;    
                
                model.MemberAddressKey = Guid.NewGuid();
                model.Status = "Active";
                model.CreateBy = (int)model.MemberId;
                model.CreateDate = DateTimeHelper.DubaiTime();
                model = await uow.MemberAddressRepository.Add(model);
                return model;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }
        public async Task<MemberAddress> Update(MemberAddress model)
        {
            try
            {
                MemberAddress memberAddress = await uow.MemberAddressRepository.GetById(model.MemberAddressId);
                if (memberAddress != null)
                {
                    if (!uow.MemberAddressRepository.Get().Any(p => p.MemberId == model.MemberId && p.Status.ToLower() == "active"))
                    {
                        memberAddress.IsDefault = true;
                    }           
                    memberAddress.CountryId = model.CountryId;
                    memberAddress.CityId = model.CityId;
                    memberAddress.AddressLine1 = model.AddressLine1;
                    memberAddress.AddressLine2 = model.AddressLine2;
                    memberAddress.NearByAddress = model.NearByAddress;
                    memberAddress.Type = model.Type;
                    memberAddress.MemberId = model.MemberId;
                    memberAddress.Status = "Active";
                    memberAddress.latitude = model.latitude;
                    memberAddress.longitude = model.longitude;
                    memberAddress.UpdateDate = DateTimeHelper.DubaiTime();
                    memberAddress.UpdateBy = (int)model.MemberId;
                    memberAddress = await uow.MemberAddressRepository.Edit(memberAddress);
                    return memberAddress;
                }
                return null;

            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }
        public async Task<MemberAddress> MakeDefaultAddress(int MemberAddressId)
        {
            try
            {
                MemberAddress memberAddress = await uow.MemberAddressRepository.Get().Where(p=> p.MemberAddressId== MemberAddressId).FirstOrDefaultAsync();
                if (memberAddress != null)
                {            
                    List<MemberAddress> list = await uow.MemberAddressRepository.Get().Where(p => p.MemberId == memberAddress.MemberId && p.MemberAddressId != MemberAddressId && p.Status.ToLower() == "active").ToListAsync();
                    foreach (MemberAddress node in list)
                    {
                        node.IsDefault = false;
                        await uow.MemberAddressRepository.Edit(node);
                    }

                    memberAddress.IsDefault=true;
                    memberAddress.Status = "Active";
                    memberAddress.CreateBy = memberAddress.CreateBy;
                    memberAddress.CreateDate = memberAddress.CreateDate;
                    memberAddress.UpdateBy = (int)memberAddress.MemberId;
                    memberAddress.UpdateDate = DateTimeHelper.DubaiTime();
                    memberAddress = await uow.MemberAddressRepository.Edit(memberAddress);
                    return memberAddress;
                }
                return null;

            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }

        public async Task<bool> Delete(int MemberAddressId)
        {
            try
            {
                MemberAddress memberAddress = await uow.MemberAddressRepository.Get().Where(p => p.MemberAddressId == MemberAddressId).FirstOrDefaultAsync();
                if (memberAddress != null)
                {
                    memberAddress.Status = "deleted";
                    memberAddress.IsDefault = false;
                    memberAddress.DeleteDate = DateTimeHelper.DubaiTime();
                    memberAddress.DeleteBy = (int)memberAddress.MemberId;
                    await uow.MemberAddressRepository.Edit(memberAddress);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return false;
            }
        }
    }
}