using Boulevard.BaseRepository;
using Boulevard.Helper;
using Boulevard.Models;
using Boulevard.RequestModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Boulevard.Service.WebAPI
{
    public class UserReviewServiceAccess
    {
        public IUnitOfWork uow;
        public UserReviewServiceAccess()
        {
            uow = new UnitOfWork();
        }

        public async Task<bool> AddUserReview(UserReviewRequest Review)
        {

            try
            {

               var userreview = new UserReview();
                userreview.UserId = Review.UserId;
                userreview.UserType = "Member";
                userreview.FeatureType = Review.FeatureType;
                userreview.FeatureTypeId = Review.FeatureTypeId;
                userreview.Rating = Review.Rating;
                userreview.Comment = Review.Comment;
                userreview.Details = Review.Details;
                userreview.Status = "Active";
                userreview.CreateDate = Helper.DateTimeHelper.DubaiTime();
                userreview.CreateBy = Review.UserId;



               var insertedUserReview =  await uow.UserReviewRepository.Add(userreview);

                if (Review.ReviewImages != null && Review.ReviewImages.Count() > 0)
                {
                    foreach (var ss in Review.ReviewImages)
                    {
                        var reviewImage = new UserReviewImage();
                        reviewImage.UserReviewId = insertedUserReview.UserReviewId;
                        reviewImage.Image = ss;
                        await uow.UserReviewImageRepository.Add(reviewImage);
                    }

                }
               

                if (Review.FeatureType == "Service")
                {
                    var result = await uow.UserReviewRepository.Get().Where(s => s.FeatureTypeId == Review.FeatureTypeId && s.FeatureType == "Service").ToListAsync();
                    var totalRating = result.Sum(s => s.Rating) / result.Count();
                    if (totalRating > 0)
                    {
                        var service = await uow.ServiceRepository.Get().Where(s => s.ServiceId == Review.FeatureTypeId).FirstOrDefaultAsync();
                        if (service != null)
                        {
                            service.Ratings = totalRating;
                            await uow.ServiceRepository.Edit(service);

                        }
                    }
                }
                else
                {
                    var result = await uow.UserReviewRepository.Get().Where(s => s.FeatureTypeId == Review.FeatureTypeId && s.FeatureType == "Product").ToListAsync();
                    var totalRating = result.Sum(s => s.Rating) / result.Count();
                    if (totalRating > 0)
                    {
                        var product = await uow.ProductRepository.Get().Where(s => s.ProductId == Review.FeatureTypeId).FirstOrDefaultAsync();
                        if (product != null)
                        {
                            product.AvgRatings = totalRating;
                            await uow.ProductRepository.Edit(product);

                        }
                    }

                }


                return true;

            }
            catch (Exception ex)
            {

                return false;
            }
        }
    }
}