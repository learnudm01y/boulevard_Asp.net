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
    public class FavouriteController : BaseController
    {
		public FavouriteServiceProductAccess _favouriteService;
		public FavouriteController()
		{
			_favouriteService = new FavouriteServiceProductAccess();
		}
		// GET: WebHtml
		[HttpPost]
		public async Task<IHttpActionResult> AddOrRemoveFavourite(FavouriteRequest request)
		{
			var result = await _favouriteService.AddOrRemoveWish(request);
			
				return SuccessMessage(result);
			
			
		}


		public async Task<IHttpActionResult> getFavouriteProducts( int memberId,int featureCategoryId)
		{
			var result = await _favouriteService.GetFavouriteProducts(memberId,featureCategoryId);
			if (result != null && result.Count() > 0)
			{
				return SuccessMessage(result);
			}
			else
			{
				return ErrorMessage("No Data Found");
			}


		}

		public async Task<IHttpActionResult> getFavouriteService(int memberId, int featureCategoryId)
		{
			var result = await _favouriteService.GetFavouriteService(memberId, featureCategoryId);
			if (result != null && result.Count() > 0)
			{
				return SuccessMessage(result);
			}
			else
			{
				return ErrorMessage("No Data Found");
			}


		}
	}
}
