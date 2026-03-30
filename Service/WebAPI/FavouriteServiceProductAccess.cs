using Boulevard.BaseRepository;
using Boulevard.Helper;
using Boulevard.Models;
using Boulevard.RequestModels;
using Boulevard.ResponseModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Boulevard.Service.WebAPI
{
	public class FavouriteServiceProductAccess
	{
		public IUnitOfWork uow;
		public FavouriteServiceProductAccess()
		{
			uow = new UnitOfWork();
		}

		public async  Task<string> AddOrRemoveWish(FavouriteRequest request)
		{

			try
			{
				if (request.ProductId > 0 && request.ServiceId == 0)
				{
					var model = await uow.FavouriteProductRepository.Get().Where(x => x.ProductId == request.ProductId && x.MemberId == request.MemberId && x.FeatureCategoryId == request.FeatureCategoryId).FirstOrDefaultAsync();

					if (model != null)
					{

						uow.FavouriteProductRepository.Remove(model);

						return "Removed from favorites";
					}
					else
					{
						model = new FavouriteProduct();
						model.MemberId = request.MemberId;
						model.ProductId = request.ProductId;
						model.Status = true;
						model.LastModified = DateTimeHelper.DubaiTime();
						model.FeatureCategoryId = request.FeatureCategoryId;

						await uow.FavouriteProductRepository.Add(model);

						return "Added to favorites";
					}
				}
				else if (request.ProductId == 0 && request.ServiceId > 0)
				{
					var model = await uow.FavouriteServiceRepository.Get().Where(x => x.ServiceId == request.ServiceId && x.MemberId == request.MemberId && x.FeatureCategoryId == request.FeatureCategoryId && x.ServiceTypeId == request.ServiceTypeId).FirstOrDefaultAsync();

					if (model != null)
					{

						uow.FavouriteServiceRepository.Remove(model);

						return "Removed from favorites";
					}
					else
					{
						model = new FavouriteService();
						model.MemberId = request.MemberId;
						model.ServiceId = request.ServiceId;
						model.ServiceTypeId = request.ServiceTypeId;
						model.Status = true;
						model.LastModified = DateTimeHelper.DubaiTime();
						model.FeatureCategoryId = request.FeatureCategoryId;

						await uow.FavouriteServiceRepository.Add(model);

						return "Added to favorites";
					}

				}
				else
				{
					return "SomeProblem Occurs";
				}
			}
			catch (Exception ex)
			{

				return "SomeProblem Occurs";
			}
		}

		public async Task<List<ProductSmallDetailsResponse>> GetFavouriteProducts(int memberId,int featureCategoryid)
		{

			try
			{
				var model = new List<ProductSmallDetailsResponse>();
				var productIds = await uow.FavouriteProductRepository.Get().Where(x => x.MemberId == memberId && x.FeatureCategoryId==featureCategoryid).Select(x => x.ProductId).ToArrayAsync();

				if (productIds.Count() >0 && productIds!=null)
				{
					foreach (var proid in productIds)
					{
						var list = await new ProductServiceAccess().getSmallDetailsProducts(proid, memberId);
						if (list != null)
						{
							model.Add(list);
						}
					}
				}

				
				return model;
			}
			catch (Exception ex)
			{

				return null;
			}
		}

		public async Task<List<SmallServiceDetailsResponse>> GetFavouriteService(int memberId, int featureCategoryid)
		{

			try
			{
				var model = new List<SmallServiceDetailsResponse>();
				var serviceTypeids = await uow.FavouriteServiceRepository.Get().Where(x => x.MemberId == memberId && x.FeatureCategoryId == featureCategoryid).Select(x => x.ServiceTypeId).ToArrayAsync();

				if (serviceTypeids.Count() > 0 || serviceTypeids != null)
				{
					foreach (var serviceTypeId in serviceTypeids)
					{
						var serviceType = await uow.ServiceTypesRepository.Get().Where(s => s.ServiceTypeId == serviceTypeId && s.Status == "Active").FirstOrDefaultAsync();
						if (serviceType != null)
						{
							var ServiceResult = await new ServiceAccess().GetSmallServices(serviceType.ServiceId, serviceType.ServiceTypeId,memberId);
							if (ServiceResult != null)
							{
								model.Add(ServiceResult);
							}
						}
					}
				}


				return model;
			}
			catch (Exception ex)
			{

				return null;
			}
		}
	}
}