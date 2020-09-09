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
    public class ShopImageController : ApiController
    {
        [Route("AddShopImages")]
        public bool AddShopImages(ShopImageDto dataDto)
        {
            if (dataDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    ShopImage addData = new ShopImage();

                    addData.Name = dataDto.Name;
                    addData.Image = dataDto.Image;
                    addData.ShopId = dataDto.ShopId;
                    addData.IsActive = true;



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
                        addData.Image = string.Concat("UploadedFiles\\" + id + ".jpg");
                    }

                    context.ShopImages.Add(addData);
                    context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        [HttpGet]
        [Route("DeleteShopImages/{id}")]
        public bool DeleteShopImages(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.ShopImages.FirstOrDefault(x => x.Id == id);
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
        [Route("ShopImageInView")]
        public DataSourceResult ShopImageInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.ShopImages.Where(x => x.IsActive == true)
                    .Select(x => new ShopImageDto
                    {

                        Id = x.Id,
                        Name = x.Name,
                        Image = x.Image,
                        IsActive = x.IsActive,
                        ShopId = x.ShopId,
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
        [Route("GetShopImage/{id}")]
        public List<ShopImageDto> GetShopImage(long id)
        {
            if (id > 0)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    var data = context.ShopImages.Where(x => x.IsActive && x.ShopId == id)
                        .Select(x => new ShopImageDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            IsActive = x.IsActive,
                            Image = x.Image,
                            ShopId = x.ShopId,
                            Shop = new ShopDto
                            {
                                Id = x.Shop.Id,
                                Name = x.Shop.Name,
                            },
                        }).OrderByDescending(x => x.Id).ToList();

                    return data;
                }
            }
            return null;
        }
    }
}
