using Boulevard.BaseRepository;
using Boulevard.Models;
using Boulevard.RequestModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Boulevard.Service.WebAPI
{
    public class MemberVehicalModelAccess
    {
        public IUnitOfWork uow;
        public MemberVehicalModelAccess()
        {
            uow = new UnitOfWork();
        }
        public async Task<List<VehicalModel>> GetAllByBrandId(int brandId,string lang="en")
        {
            try
            {
                var result = await uow.VehicalModelRepository.Get().Where(b => b.BrandId == brandId && b.Status == "Active").ToListAsync();

                if (result != null && result.Count() > 0)
                {
                    foreach (var res in result)
                    {
                        res.VehicalModelName= lang == "en"?res.VehicalModelName:res.VehicalModelNameAr;
                        res.ModelDetails = lang == "en" ? res.ModelDetails : res.ModelDetailsAr;
                       
                    }
                }
                return result;

            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }

        public async Task<bool> CreateVehicalMemberInfo(MemberVehicalInfoRequest info)
        {
            try
            {
                if (info.MemberVehicalInfoId == 0)
                {
                    var result = new MemberVehicalInfo();
					result.BrandId = info.BrandId;
					result.VehicalModelId = info.VehicalModelId;
					result.Year = info.Year;
					result.PlateNo = info.PlateNo;
                    result.MemberId = info.MemberId;
                    result.Status = "Active";
					result.CreateDate = Helper.DateTimeHelper.DubaiTime();
					result.CreateBy = Convert.ToInt32(info.MemberId);
                    await uow.MemberVehicalInfoRepository.Add(result);
					return true;

				}
                else
                {
                    var result = await uow.MemberVehicalInfoRepository.Get().Where(s => s.MemberVehicalInfoId == info.MemberVehicalInfoId).FirstOrDefaultAsync();
                    if (result != null)
                    {
                        result.BrandId = info.BrandId;
                        result.VehicalModelId = info.VehicalModelId;
                        result.Year = info.Year;
                        result.PlateNo = info.PlateNo;
						result.UpdateDate = Helper.DateTimeHelper.DubaiTime();
						result.UpdateBy = Convert.ToInt32(info.MemberId);
                        await uow.MemberVehicalInfoRepository.Edit(result);
                        return true;
					}
                }
                return false;
               
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return false;
            }
        }




        public async Task<List<MemberVehicalInfo>> GetAllByMemberId(int memberId,string lang="en")
        {
            try
            {
                var result =  await uow.MemberVehicalInfoRepository.Get().Where(b => b.MemberId == memberId && b.Status == "Active").ToListAsync();
                if (result != null && result.Count() > 0)
                {
                    foreach (var res in result)
                    {
                        if (res.VehicalModel != null)
                        {
                            res.VehicalModel.VehicalModelName = lang == "en" ? res.VehicalModel.VehicalModelName : res.VehicalModel.VehicalModelNameAr;
                            res.VehicalModel.ModelDetails = lang == "en" ? res.VehicalModel.ModelDetails : res.VehicalModel.ModelDetailsAr;
                        }
                       

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }
    }
}
