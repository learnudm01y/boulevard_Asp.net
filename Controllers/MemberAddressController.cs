using Boulevard.Models;
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
    public class MemberAddressController : BaseController
    {
        public MemberAddressAccess _memberAddressAccess;
        public MemberAddressController()
        {
            _memberAddressAccess = new MemberAddressAccess();
        }
        // GET: WebHtml
        public async Task<IHttpActionResult> GetMemberAddress(int memberId)
        {
            var result = await _memberAddressAccess.GetMemberAddress(memberId);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessage("unsuccessful..!");
            }
        }
        public async Task<IHttpActionResult> InsertMemberAddress(MemberAddress model)
        {
            var result = await _memberAddressAccess.Insert(model);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessage("unsuccessful..!");
            }
        }

        public async Task<IHttpActionResult> UpdateMemberAddress(MemberAddress model)
        {
            var result = await _memberAddressAccess.Update(model);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessage("unsuccessful..!");
            }
        }
        public async Task<IHttpActionResult> MakeDefaultAddress(int memberAddressId)
        {
            var result = await _memberAddressAccess.MakeDefaultAddress(memberAddressId);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessage("unsuccessful..!");
            }
        }

        public async Task<IHttpActionResult> RemoveAddress(int memberAddressId)
        {
            var result = await _memberAddressAccess.Delete(memberAddressId);
            if (result)
            {
                return SuccessMessage("successful..!");
            }
            else
            {
                return ErrorMessage("unsuccessful..!");
            }
        }


    }
}
