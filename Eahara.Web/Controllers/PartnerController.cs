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
    public class PartnerController : ApiController
    {
        [HttpPost]
        [Route("AddPartner")]
        public bool AddPartner(PartnerDto dataDto)
        {
            if (dataDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (dataDto.Id > 0)
                    {
                        var data = context.Partners.FirstOrDefault(x => x.Id == dataDto.Id);
                        if (data != null)
                        {
                            data.Name = dataDto.Name;

                            context.Entry(data).Property(x => x.Name).IsModified = true;
                            //context.Entry(data).Property(x => x.Image).IsModified = true;

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
                        Partner item = new Partner();

                        item.Name = dataDto.Name;
                        //item.Image = dataDto.Image;
                        item.IsActive = true;
                        context.Partners.Add(item);


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


                        context.SaveChanges();
                        return true;
                    }

                }

            }
            return false;
        }

        [HttpGet]
        [Route("DeletePartner/{id}")]
        public bool DeletePartner(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.Partners.FirstOrDefault(x => x.Id == id);
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
        [Route("PartnersInView")]
        public DataSourceResult PartnersInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.Partners.Where(x => x.IsActive == true)
                    .Select(x => new PartnerDto
                    {

                        Id = x.Id,
                        Name = x.Name,
                        Image = x.Image,
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
        [Route("PartnersInHome/{id}")]
        public List<PartnerDto> PartnersInHome(int id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Partners.Where(x => x.IsActive)
                    .Select(x => new PartnerDto
                    {
                        Id = x.Id,
                        Name = x.Name, 
                        Image = x.Image,
                    }).OrderByDescending(x => x.Id).Skip(id).Take(6).ToList();

                return data;
            }
        }

    }
}