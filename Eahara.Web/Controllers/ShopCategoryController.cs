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
    public class ShopCategoryController : ApiController
    {
        [HttpPost]
        [Route("AddShopCategory")]
        public bool AddShopCategory(ShopDto shopDto)
        {
            if (shopDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (shopDto.Id > 0)
                    {
                        var data = context.ShopCategories.FirstOrDefault(x => x.Id == shopDto.Id);
                        if (data != null)
                        {
                            data.Name = shopDto.Name;
                           // data.Image = shopDto.Image;
                            context.Entry(data).Property(x => x.Name).IsModified = true;
                            //context.Entry(data).Property(x => x.Image).IsModified = true;

                            if (shopDto.Image != null && shopDto.Image != "" && data.Image != shopDto.Image && !shopDto.Image.Contains("http"))
                            {
                                Guid id = Guid.NewGuid();
                                var imgData = shopDto.Image.Substring(shopDto.Image.IndexOf(",") + 1);
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
                        ShopCategory shop = new ShopCategory();

                        shop.Name = shopDto.Name;
                        shop.IsActive = true;

                        if (shopDto.Image != null && shopDto.Image != "")
                        {
                            Guid id = Guid.NewGuid();
                            var imgData = shopDto.Image.Substring(shopDto.Image.IndexOf(",") + 1);
                            byte[] bytes = Convert.FromBase64String(imgData);
                            Image image;
                            using (MemoryStream ms = new MemoryStream(bytes))
                            {
                                image = Image.FromStream(ms);
                            }
                            Bitmap b = new Bitmap(image);
                            string filePath = System.Web.HttpContext.Current.Server.MapPath("~") + "UploadedFiles\\" + id + ".jpg";
                            b.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                            shop.Image = string.Concat("UploadedFiles\\" + id + ".jpg");
                        }

                        context.ShopCategories.Add(shop);
                        context.SaveChanges();
                        return true;
                    }

                }

            }
            return false;
        }

        [HttpPost]
        [Route("ShopCategorysInView")]
        public DataSourceResult ShopCategorysInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.ShopCategories.Where(x => x.IsActive == true)
                    .Select(x => new ShopCategoryDto
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
        [Route("DeleteShopCategoryById/{id}")]
        public bool DeleteShopCategoryById(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.ShopCategories.FirstOrDefault(x => x.Id == id);
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
        [Route("ShopCategoryInDropdown")]
        public List<ShopCategoryDto> ShopCategoryInDropdown()
        {
            List<ShopCategoryDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.ShopCategories.Where(x => x.IsActive == true)
                    .Select(x => new ShopCategoryDto
                    {
                        Id = x.Id,
                        IsActive = x.IsActive,
                        Name = x.Name,
                        Image = x.Image,
                        IsChecked = false,
                       
                    }).ToList();

                DtoList = data;
            }
            return DtoList;
        }

    }
}
