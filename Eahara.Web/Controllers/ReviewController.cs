using Eahara.Model;
using Eahara.Web.Dtos;
using Kendo.DynamicLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Eahara.Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReviewController : ApiController
    {
        [Route("AddReview")]
        public bool AddReview(ReviewDto dataDto)
        {
            if (dataDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    var old = context.Reviews.FirstOrDefault(x => x.IsActive && x.ShopId == dataDto.ShopId && x.MobileNo == dataDto.MobileNo);

                    if (old != null)
                    {
                        old.Rating = dataDto.Rating;
                        old.Description = dataDto.Description;

                        context.Entry(old).Property(x => x.Rating).IsModified = true;
                        context.Entry(old).Property(x => x.Description).IsModified = true;

                        context.SaveChanges();
                        CalcAvgRating(dataDto.ShopId);
                        return true;
                    }

                    Review addData = new Review();

                    addData.Name = dataDto.Name;
                    addData.MobileNo = dataDto.MobileNo;
                    addData.EmailId = dataDto.EmailId;
                    addData.Rating = dataDto.Rating;
                    addData.ShopId = dataDto.ShopId;
                    addData.Description = dataDto.Description;
                    addData.IsActive = true;
                    context.Reviews.Add(addData);
                    context.SaveChanges();
                    CalcAvgRating(dataDto.ShopId);
                    return true;
                }
            }
            return false;
        }

        public bool CalcAvgRating(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
              var shop = context.Shops.FirstOrDefault(x => x.Id == id);
                var ratings = context.Reviews.Where(x => x.IsActive && x.ShopId == id).Count();
                if (ratings > 0)
                {
                    var tot = context.Reviews.Where(x => x.IsActive && x.ShopId == id).Sum(x=> x.Rating);
                    var avg = tot / ratings;
                    shop.AverageRating = avg.ToString();
                    context.Entry(shop).Property(x => x.AverageRating).IsModified = true;
                    context.SaveChanges();
                }
            }            
            return true;
        }

        [HttpGet]
        [Route("DeleteReviews/{id}")]
        public bool DeleteReviews(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.Reviews.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {
                        Delete.IsActive = false;
                        context.Entry(Delete).Property(x => x.IsActive).IsModified = true;
                    }
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }

        [HttpPost]
        [Route("ReviewsInView")]
        public DataSourceResult ReviewsInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.Reviews.Where(x => x.IsActive == true)
                    .Select(x => new ReviewDto
                    {

                        Id = x.Id,
                        Description = x.Description,
                        Rating = x.Rating,
                        IsActive = x.IsActive,
                        ShopId = x.ShopId,
                        Name = x.Name,
                        MobileNo = x.MobileNo,
                        EmailId = x.EmailId,
                        Shop = new ShopDto
                        {
                            Id = x.Shop.Id,
                            Name = x.Shop.Name,
                        },
                    }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }

        [HttpGet]
        [Route("getShopReviews/{id}")]
        public List<ReviewDto> getShopReviews(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataList = context.Reviews.Where(x => x.IsActive == true && x.ShopId == id)
                    .Select(x => new ReviewDto
                    {

                        Id = x.Id,
                        Description = x.Description,
                        Rating = x.Rating,
                        IsActive = x.IsActive,
                        Name = x.Name,
                        MobileNo = x.MobileNo,
                        EmailId = x.EmailId,
                        ShopId = x.ShopId,
                        Shop = new ShopDto
                        {
                            Id = x.Shop.Id,
                            Name = x.Shop.Name,
                        },
                    }).OrderByDescending(x => x.Id).ToList();

                return dataList;
            }
        }

        [HttpGet]
        [Route("ReviewsInDashboard/{id}")]
        public List<ReviewDto> ReviewsInDashboard(int id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataList = context.Reviews.Where(x => x.IsActive)
                    .Select(x => new ReviewDto
                    {

                        Id = x.Id,
                        Description = x.Description,
                        Rating = x.Rating,
                        IsActive = x.IsActive,
                        Name = x.Name,
                        MobileNo = x.MobileNo,
                        EmailId = x.EmailId,
                        ShopId = x.ShopId,
                        Shop = new ShopDto
                        {
                            Id = x.Shop.Id,
                            Name = x.Shop.Name,
                        },
                    }).OrderByDescending(x => x.Id).Skip(id).Take(3).ToList();

                return dataList;
            }
        }
    }
}
