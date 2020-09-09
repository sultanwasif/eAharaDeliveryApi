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
    public class MEDBrandController : ApiController
    {
        [HttpPost]
        [Route("AddMEDBrand")]
        public bool AddMEDBrand(MEDBrandDto itemDto)
        {
            if (itemDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (itemDto.Id > 0)
                    {
                        var data = context.MEDBrands.FirstOrDefault(x => x.Id == itemDto.Id);
                        if (data != null)
                        {
                            data.Name = itemDto.Name;
                            // data.Image = itemDto.Image;
                            context.Entry(data).Property(x => x.Name).IsModified = true;
                            //context.Entry(data).Property(x => x.Image).IsModified = true;

                            if (itemDto.Image != null && itemDto.Image != "" && data.Image != itemDto.Image && !itemDto.Image.Contains("http"))
                            {
                                Guid id = Guid.NewGuid();
                                var imgData = itemDto.Image.Substring(itemDto.Image.IndexOf(",") + 1);
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
                        MEDBrand item = new MEDBrand();

                        item.Name = itemDto.Name;
                        item.IsActive = true;
                        context.MEDBrands.Add(item);


                        if (itemDto.Image != null && itemDto.Image != "")
                        {
                            Guid id = Guid.NewGuid();
                            var imgData = itemDto.Image.Substring(itemDto.Image.IndexOf(",") + 1);
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

        [HttpPost]
        [Route("MEDBrandsInView")]
        public DataSourceResult MEDBrandsInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.MEDBrands.Where(x => x.IsActive == true)
                    .Select(x => new MEDBrandDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Image = x.Image,
                    }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }

        [HttpGet]
        [Route("DeleteMEDBrandById/{id}")]
        public bool DeleteMEDBrandById(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.MEDBrands.FirstOrDefault(x => x.Id == id);
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


        [HttpGet]
        [Route("MEDBrandInDropdown")]
        public List<MEDBrandDto> MEDBrandInDropdown()
        {
            List<MEDBrandDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDBrands.Where(x => x.IsActive == true)
                    .Select(x => new MEDBrandDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Image = x.Image,
                    }).ToList();

                DtoList = data;
            }
            return DtoList;
        }
    }
}
