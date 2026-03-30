using Boulevard.RequestModels;
using Boulevard.Service.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Boulevard.Controllers
{
    public class CommunitySetupController : BaseController
    {

        public CommunitySetupAccess _communitySetupAccess;
        public CommunitySetupController()
        {
            _communitySetupAccess = new CommunitySetupAccess();
        }
        // GET: CommunitySetup

        [System.Web.Http.HttpGet]
        public async Task<IHttpActionResult> Index()
        {
            try
            {
                var result = await _communitySetupAccess.GetAll();
                if (result != null)
                {
                    return SuccessMessage(result);
                }
                else
                {
                    return ErrorMessage("An error occured, please try again later!");

                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<IHttpActionResult> Insert(CommunitySetupRequest communitySetupRequest)
        {
            try
            {
                var result = await _communitySetupAccess.CreateCommunitySetup(communitySetupRequest);
                if (result != null)
                {
                    return SuccessMessage(result);
                }
                else
                {
                    return ErrorMessage("An error occured, please try again later!");

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}