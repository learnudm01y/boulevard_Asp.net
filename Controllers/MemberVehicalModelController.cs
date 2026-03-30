using Boulevard.Models;
using Boulevard.RequestModels;
using Boulevard.Service.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Boulevard.Controllers
{
    public class MemberVehicalModelController : BaseController
    {
        private readonly MemberVehicalModelAccess _memberVehicalModelAccess;
        public MemberVehicalModelController()
        {
            _memberVehicalModelAccess = new MemberVehicalModelAccess();
        }
        // GET: MemberVehicalModel
        public async Task<IHttpActionResult> GetVehicalModelbyBrandId(int brandId,string lang="en")
        {
            var result = await _memberVehicalModelAccess.GetAllByBrandId(brandId);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }
        
        public async Task<IHttpActionResult> GetMemberInfobyMemberId(int memberId)
        {
            var result = await _memberVehicalModelAccess.GetAllByMemberId(memberId);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }

		public async Task<IHttpActionResult> CreatemembervehicalInfo(MemberVehicalInfoRequest info)
		{
			var result = await _memberVehicalModelAccess.CreateVehicalMemberInfo(info);
			if (result == true)
			{
				return SuccessMessage(result);
			}
			else
			{
				return ErrorMessage("Something is wrong");
			}
		}
	}
}