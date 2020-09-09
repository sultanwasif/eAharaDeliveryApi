using Eahara.Model;
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
    public class MEDOfferController : ApiController
    {
        [HttpPost]
        [Route("AddMEDOffers")]
        public bool AddMEDOffers(MEDOfferDto dataDto)
        {
            if (dataDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (dataDto.Id > 0)
                    {
                        var data = context.MEDOffers.FirstOrDefault(x => x.Id == dataDto.Id);
                        if (data != null)
                        {
                            data.Title = dataDto.Title;
                            data.Percentage = dataDto.Percentage;
                            data.IsPercentage = dataDto.IsPercentage;
                            data.MEDShopId = dataDto.MEDShopId;

                            context.Entry(data).Property(x => x.Title).IsModified = true;
                            context.Entry(data).Property(x => x.Percentage).IsModified = true;
                            context.Entry(data).Property(x => x.MEDShopId).IsModified = true;
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

                            var items = context.MEDItems.Where(x => x.IsActive && x.MEDOfferId == data.Id).ToList();
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
                        MEDOffer item = new MEDOffer();

                        item.Title = dataDto.Title;
                        item.Percentage = dataDto.Percentage;
                        item.IsPercentage = dataDto.IsPercentage;
                        item.MEDShopId = dataDto.MEDShopId;
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

                        context.MEDOffers.Add(item);
                        context.SaveChanges();
                        return true;
                    }

                }

            }
            return false;
        }

        [HttpGet]
        [Route("DeleteMEDOffers/{id}")]
        public bool DeleteMEDOffers(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.MEDOffers.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {
                        Delete.IsActive = false;
                        context.Entry(Delete).Property(x => x.IsActive).IsModified = true;

                        var items = context.MEDItems.Where(x => x.IsActive && x.MEDOfferId == id).ToList();
                        foreach (var i in items)
                        {
                            i.MEDOfferId = null;
                            context.Entry(i).Property(x => x.MEDOfferId).IsModified = true;
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

        [HttpPost]
        [Route("MEDOffersInView")]
        public DataSourceResult MEDOffersInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.MEDOffers.Where(x => x.IsActive == true)
                    .Select(x => new MEDOfferDto
                    {

                        Id = x.Id,
                        Title = x.Title,
                        Image = x.Image,
                        Percentage = x.Percentage,
                        MEDShopId = x.MEDShopId,
                        IsActive = x.IsActive,
                        MEDShop = new MEDShopDto
                        {
                            Id = x.MEDShop != null ? x.MEDShop.Id : 0,
                            Name = x.MEDShop != null ? x.MEDShop.Name : "",
                        },
                        IsPercentage = x.IsPercentage,
                    }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }

        [HttpGet]
        [Route("MEDOfferById/{id}")]
        public MEDOfferDto MEDOfferById(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.MEDOffers.Where(x => x.IsActive == true && x.Id == id)
                    .Select(x => new MEDOfferDto
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Image = x.Image,
                        Percentage = x.Percentage,
                        MEDShopId = x.MEDShopId,
                        IsActive = x.IsActive,
                        MEDShop = new MEDShopDto
                        {
                            Id = x.MEDShop != null ? x.MEDShop.Id : 0,
                            Name = x.MEDShop != null ? x.MEDShop.Name : "",
                        },
                        IsPercentage = x.IsPercentage,
                    }).FirstOrDefault();

                return dataSourceResult;
            }
        }

        [HttpGet]
        [Route("MEDOffersInDropdown")]
        public List<MEDOfferDto> MEDOffersInDropdown()
        {
            List<MEDOfferDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDOffers.Where(x => x.IsActive == true)
                    .Select(x => new MEDOfferDto
                    {
                        Id = x.Id,
                        Title = x.Title,
                        IsActive = x.IsActive,
                        Image = x.Image,
                    }).ToList();

                DtoList = data;
            }
            return DtoList;
        }


        [HttpGet]
        [Route("GetMEDOffersInHome")]
        public List<MEDOfferDto> GetMEDOffersInHome()
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDOffers.Where(x => x.IsActive)
                    .Select(x => new MEDOfferDto
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Image = x.Image,
                        MEDShopId = x.MEDShopId,
                        Percentage = x.Percentage,
                        IsPercentage = x.IsPercentage,
                    }).OrderByDescending(x => x.Id).Take(12).ToList();

                return data;
            }
        }


        [HttpGet]
        [Route("MEDOffersInDropdownByMedShop/{id}")]
        public List<MEDOfferDto> MEDOffersInDropdownByMedShop(long id)
        {
            List<MEDOfferDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDOffers.Where(x => x.IsActive == true && x.MEDShopId == id)
                    .Select(x => new MEDOfferDto
                    {
                        Id = x.Id,
                        Title = x.Title,
                        IsActive = x.IsActive,
                        Image = x.Image,
                    }).ToList();

                DtoList = data;
            }
            return DtoList;
        }


    }
}
