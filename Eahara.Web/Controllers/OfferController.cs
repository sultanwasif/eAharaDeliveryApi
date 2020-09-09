using Eahara.Model;
using Eahara.Web.Filter;
using Eahara.Web.Dtos;
using Kendo.DynamicLinq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Eahara.Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class OfferController : ApiController
    {
        [HttpPost]
        [Route("AddOffers")]
        public bool AddOffers(OfferDto dataDto)
        {
            if (dataDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (dataDto.Id > 0)
                    {
                        var data = context.Offers.FirstOrDefault(x => x.Id == dataDto.Id);
                        if (data != null)
                        {
                            data.Tittle = dataDto.Tittle;
                            data.Percentage = dataDto.Percentage;
                            data.ShopId = dataDto.ShopId;
                            data.IsPercentage = dataDto.IsPercentage;
                            
                            context.Entry(data).Property(x => x.Tittle).IsModified = true;
                            context.Entry(data).Property(x => x.ShopId).IsModified = true;
                            context.Entry(data).Property(x => x.Percentage).IsModified = true;
                            context.Entry(data).Property(x => x.IsPercentage).IsModified = true;

                            if (dataDto.Image != null && dataDto.Image != "" && data.Image != dataDto.Image && !dataDto.Image.Contains("http"))
                            {
                                Guid id = Guid.NewGuid();
                                var imgData = dataDto.Image.Substring(dataDto.Image.IndexOf(",") + 1);
                                byte[] bytes = Convert.FromBase64String(imgData);
                                Image image;
                                using (MemoryStream ms = new MemoryStream(bytes))
                                {
                                    image = Image.FromStream(ms);
                                }
                                Bitmap b = new Bitmap(image);
                                string filePath = System.Web.HttpContext.Current.Server.MapPath("~") + "UploadedFiles\\" + id + ".jpg";
                                b.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                                data.Image = string.Concat("UploadedFiles\\" + id + ".jpg");
                                context.Entry(data).Property(x => x.Image).IsModified = true;
                            }

                            var items = context.Items.Where(x => x.IsActive && x.OfferId == data.Id).ToList();
                            foreach (var i in items)
                            {
                                if (data.IsPercentage)
                                {
                                    i.OfferPrice = i.Price - (i.Price * data.Percentage) / 100;
                                }
                                else
                                {
                                    i.OfferPrice = i.Price - data.Percentage;
                                }
                                context.Entry(i).Property(x => x.OfferPrice).IsModified = true;
                            }


                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        Offer item = new Offer();

                        item.Tittle = dataDto.Tittle;
                        item.Percentage = dataDto.Percentage;
                        item.ShopId = dataDto.ShopId;
                        item.IsPercentage = dataDto.IsPercentage;
                        item.IsActive = true;
                        
                        if (dataDto.Image != null && dataDto.Image != "")
                        {
                            Guid id = Guid.NewGuid();
                            var imgData = dataDto.Image.Substring(dataDto.Image.IndexOf(",") + 1);
                            byte[] bytes = Convert.FromBase64String(imgData);
                            Image image;
                            using (MemoryStream ms = new MemoryStream(bytes))
                            {
                                image = Image.FromStream(ms);
                            }
                            Bitmap b = new Bitmap(image);
                            string filePath = System.Web.HttpContext.Current.Server.MapPath("~") + "UploadedFiles\\" + id + ".jpg";
                            b.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                            item.Image = string.Concat("UploadedFiles\\" + id + ".jpg");
                        }

                        context.Offers.Add(item);
                        context.SaveChanges();
                        return true;
                    }

                }

            }
            return false;
        }
        
        [HttpGet]
        [Route("DeleteOffers/{id}")]
        public bool DeleteOffers(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.Offers.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {
                        Delete.IsActive = false;
                        context.Entry(Delete).Property(x => x.IsActive).IsModified = true;

                        var items = context.Items.Where(x => x.IsActive && x.OfferId == id).ToList();
                        foreach(var i in items)
                        {
                            i.OfferId = null;
                            context.Entry(i).Property(x => x.OfferId).IsModified = true;
                            i.OfferPrice = i.Price;
                            context.Entry(i).Property(x => x.OfferPrice).IsModified = true;
                        }

                    }
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }

        [OrangeAuthorizationFilter]
        [HttpPost]
        [Route("OffersInView")]
        public DataSourceResult OffersInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;
                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var query = context.Offers.Where(x => x.IsActive == true);
                if (user.Role == "Employee")
                {
                    if (user.Employee != null)
                    {
                        query = query.Where(x => x.Shop.LocationId == user.Employee.LocationId);
                    }
                }
                var dataSourceResult = query
                    .Select(x => new OfferDto
                    {

                        Id = x.Id,
                        Tittle = x.Tittle,
                        Image = x.Image,
                        Percentage = x.Percentage,
                        IsActive = x.IsActive,
                        IsPercentage = x.IsPercentage,
                        ShopId = x.ShopId,
                        Shop = new ShopDto
                        {
                            Id = x.Shop != null ? x.Shop.Id : 0,
                            Name = x.Shop != null ? x.Shop.Name : "",
                        },
                    }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }

        [HttpPost]
        [Route("ShopOffersInView")]
        public DataSourceResult ShopOffersInView(KendoFilterDto Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.Offers.Where(x => x.IsActive == true && x.ShopId==Request.ShopId)
                    .Select(x => new OfferDto
                    {

                        Id = x.Id,
                        Tittle = x.Tittle,
                        Image = x.Image,
                        Percentage = x.Percentage,
                        IsActive = x.IsActive,
                        ShopId = x.ShopId,
                        IsPercentage = x.IsPercentage,
                        Shop = new ShopDto
                        {
                            Id = x.Shop != null ? x.Shop.Id : 0,
                            Name = x.Shop != null ? x.Shop.Name : "",
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
        [Route("GetOffers/{Id}")]
        public List<OfferDto> GetOffers(long Id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Offers.Where(x => x.IsActive && (x.Shop == null || x.Shop != null ? x.Shop.LocationId == Id : x.Shop == null))
                    .Select(x => new OfferDto
                    {
                        Id = x.Id,
                        Tittle = x.Tittle,
                        Image = x.Image,
                        Percentage = x.Percentage,
                        ShopId = x.ShopId,
                        IsPercentage = x.IsPercentage,
                        Shop = new ShopDto
                        {
                            Id = x.Shop != null ? x.Shop.Id : 0,
                            Name = x.Shop != null ? x.Shop.Name : "",
                        },
                    }).OrderByDescending(x => x.Id).ToList();

                return data;
            }
        }

        [HttpGet]
        [Route("GetOffers")]
        public List<OfferDto> GetOffers2()
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Offers.Where(x => x.IsActive)
                    .Select(x => new OfferDto
                    {
                        Id = x.Id,
                        Tittle = x.Tittle,
                        Image = x.Image,
                        Percentage = x.Percentage,
                        ShopId = x.ShopId,
                        IsPercentage = x.IsPercentage,
                        Shop = new ShopDto
                        {
                            Id = x.Shop != null ? x.Shop.Id : 0,
                            Name = x.Shop != null ? x.Shop.Name : "",
                        },
                    }).OrderByDescending(x => x.Id).ToList();

                return data;
            }
        }

        [HttpGet]
        [Route("GetShopOffers/{id}")]
        public List<OfferDto> GetShopOffers(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Offers.Where(x => x.IsActive && x.ShopId == id)
                    .Select(x => new OfferDto
                    {
                        Id = x.Id,
                        Tittle = x.Tittle,
                        Image = x.Image,
                        Percentage = x.Percentage,
                        ShopId = x.ShopId,
                        IsPercentage = x.IsPercentage,
                        Shop = new ShopDto
                        {
                            Id = x.Shop != null ? x.Shop.Id : 0,
                            Name = x.Shop != null ? x.Shop.Name : "",
                        },
                    }).OrderByDescending(x => x.Id).ToList();

                return data;
            }
        }

        [HttpGet]
        [Route("GetOfferyId/{id}")]
        public OfferDto GetOfferyId(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Offers.Where(x => x.Id == id)
                    .Select(x => new OfferDto
                    {
                        Id = x.Id,
                        Tittle = x.Tittle,
                        Image = x.Image,
                        Percentage = x.Percentage,
                        ShopId = x.ShopId,
                        IsPercentage = x.IsPercentage,
                        Shop = new ShopDto
                        {
                            Id = x.Shop != null ? x.Shop.Id : 0,
                            Name = x.Shop != null ? x.Shop.Name : "",
                        },
                    }).FirstOrDefault();

                return data;
            }
        }

        [HttpGet]
        [Route("GetOffersInHome/{Id}")]
        public List<OfferDto> GetOffersInHome(long Id)
        {
            using (EAharaDB context = new EAharaDB())
            {

                var data = context.Offers.Where(x => x.IsActive && (x.Shop == null || x.Shop != null ? x.Shop.LocationId == Id : x.Shop == null))
                    .Select(x => new OfferDto
                    {
                        Id = x.Id,
                        Tittle = x.Tittle,
                        Image = x.Image,
                        Percentage = x.Percentage,
                        ShopId = x.ShopId,
                        IsPercentage = x.IsPercentage,
                        Shop = new ShopDto
                        {
                            Id = x.Shop != null ? x.Shop.Id : 0,
                            Name = x.Shop != null ? x.Shop.Name : "",
                        },
                    }).OrderByDescending(x => x.Id).Take(12).ToList();

                return data;
            }
        }

        [HttpGet]
        [Route("GetOffersInHome")]
        public List<OfferDto> GetOffersInHome2()
        {
            using (EAharaDB context = new EAharaDB())
            {

                var data = context.Offers.Where(x => x.IsActive )
                    .Select(x => new OfferDto
                    {
                        Id = x.Id,
                        Tittle = x.Tittle,
                        Image = x.Image,
                        Percentage = x.Percentage,
                        ShopId = x.ShopId,
                        IsPercentage = x.IsPercentage,
                        Shop = new ShopDto
                        {
                            Id = x.Shop != null ? x.Shop.Id : 0,
                            Name = x.Shop != null ? x.Shop.Name : "",
                        },
                    }).OrderByDescending(x => x.Id).Take(12).ToList();

                return data;
            }
        }
    }
}
