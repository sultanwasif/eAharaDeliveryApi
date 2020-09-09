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
    public class PromoOfferController : ApiController
    {

        [HttpPost]
        [Route("AddPromoOffers")]
        public bool AddPromoOffers(PromoOfferDto dataDto)
        {
            if (dataDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (dataDto.Id > 0)
                    {
                        var data = context.PromoOffers.FirstOrDefault(x => x.Id == dataDto.Id);
                        if (data != null)
                        {
                            data.Tittle = dataDto.Tittle;
                            data.Value = dataDto.Value;
                            data.IsPercentage = dataDto.IsPercentage;
                            data.Code = dataDto.Code;
                            data.Count = dataDto.Count;
                            data.MaxValue = dataDto.MaxValue;

                            context.Entry(data).Property(x => x.Value).IsModified = true;
                            context.Entry(data).Property(x => x.Tittle).IsModified = true;
                            context.Entry(data).Property(x => x.IsPercentage).IsModified = true;
                            context.Entry(data).Property(x => x.Code).IsModified = true;
                            context.Entry(data).Property(x => x.MaxValue).IsModified = true;
                            context.Entry(data).Property(x => x.Count).IsModified = true;

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



                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        PromoOffer item = new PromoOffer();

                        item.Tittle = dataDto.Tittle;
                        item.Code = dataDto.Code;
                        item.Value = dataDto.Value;
                        item.Count = dataDto.Count;
                        item.IsPercentage = dataDto.IsPercentage;
                        item.MaxValue = dataDto.MaxValue;
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

                        context.PromoOffers.Add(item);
                        context.SaveChanges();
                        return true;
                    }

                }

            }
            return false;
        }

        [HttpGet]
        [Route("DeletePromoOffers/{id}")]
        public bool DeletePromoOffers(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.PromoOffers.FirstOrDefault(x => x.Id == id);
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
        [Route("PromoOffersInView")]
        public DataSourceResult OffersInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.PromoOffers.Where(x => x.IsActive == true)
                    .Select(x => new PromoOfferDto
                    {

                        Id = x.Id,
                        Tittle = x.Tittle,
                        Image = x.Image,
                        Value = x.Value,
                        Count = x.Count,
                        IsPercentage = x.IsPercentage,
                        MaxValue = x.MaxValue,
                        Code = x.Code,
                        IsActive = x.IsActive,
                    }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }


        [HttpGet]
        [Route("GetPromoOffers")]
        public List<PromoOfferDto> GetPromoOffers()
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.PromoOffers.Where(x => x.IsActive)
                    .Select(x => new PromoOfferDto
                    {
                        Id = x.Id,
                        Tittle = x.Tittle,
                        Image = x.Image,
                        Code = x.Code,
                        Count = x.Count,
                        MaxValue = x.MaxValue,
                        IsPercentage = x.IsPercentage,
                        Value = x.Value,
                    }).OrderByDescending(x => x.Id).ToList();

                return data;
            }
        }

        [HttpGet]
        [Route("PromoOffersByCusId/{id}")]
        public List<PromoOfferDto> PromoOffersByCusId(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.PromoOffers.Where(x => x.IsActive && x.Count > x.CustomerOffers.Where(y=>y.CustomerId == id).Count())
                    .Select(x => new PromoOfferDto
                    {
                        Id = x.Id,
                        Tittle = x.Tittle,
                        Image = x.Image,
                        Code = x.Code,
                        Count = x.Count,
                        IsPercentage = x.IsPercentage,
                        MaxValue = x.MaxValue,
                        Value = x.Value,
                        IsSelected = false,
                    }).OrderByDescending(x => x.Id).ToList();

                return data;
            }
        }

        [HttpGet]
        [Route("CheckPromoOffersByCusId/{code}/{cusid}")]
        public PromoOfferDto CheckPromoOffersByCusId(string code, long cusid)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.PromoOffers.FirstOrDefault(x => x.IsActive && x.Code == code && x.Count > x.CustomerOffers.Where(y => y.CustomerId == cusid).Count());

                PromoOfferDto poff = new PromoOfferDto();

                if (data != null)
                {
                    poff.Id = data.Id;
                    poff.Tittle = data.Tittle;
                    poff.Image = data.Image;
                    poff.Code = data.Code;
                    poff.Count = data.Count;
                    poff.MaxValue = data.MaxValue;
                    poff.IsPercentage = data.IsPercentage;
                    poff.Value = data.Value;
                    poff.IsSelected = false;
                    return poff;
                }
                else
                {
                    var data2 = context.PromoOffers.FirstOrDefault(x => x.IsActive && x.Code == code &&  x.CustomerOffers.Where(y => y.CustomerId == cusid).Count() > 0);

                    if (data2 != null)
                    {
                        poff.Id = 0;
                        return poff;
                    }
                    else
                    {
                        return null;
                    }
                    
                }
                
            }
        }

    }
}
