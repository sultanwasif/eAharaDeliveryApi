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
    public class ShopMenuController : ApiController
    {
        [Route("AddShopMenu")]
        public bool AddShopMenu(ShopMenuDto dataDto)
        {
            if (dataDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    ShopMenu addData = new ShopMenu();

                    addData.Tittle = dataDto.Tittle;
                    addData.Image = dataDto.Image;
                    addData.ShopId = dataDto.ShopId;
                    addData.IsActive = true;
                    context.ShopMenus.Add(addData);


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

                    context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        [HttpGet]
        [Route("DeleteShopMenu/{id}")]
        public bool DeleteShopMenu(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.ShopMenus.FirstOrDefault(x => x.Id == id);
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
        [Route("ShopMenuInView")]
        public DataSourceResult ShopMenuInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.ShopMenus.Where(x => x.IsActive == true)
                    .Select(x => new ShopMenuDto
                    {

                        Id = x.Id,
                        Tittle = x.Tittle,
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
        [Route("GetShopMenu/{id}")]
        public List<ShopMenuDto> GetShopMenu(long id)
        {
            if (id > 0)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    var data = context.ShopMenus.Where(x => x.IsActive && x.ShopId == id)
                        .Select(x => new ShopMenuDto
                        {
                            Id = x.Id,
                            Tittle = x.Tittle,
                            Image = x.Image,
                            IsActive = x.IsActive,
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
