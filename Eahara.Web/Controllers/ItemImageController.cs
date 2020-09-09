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
    public class ItemImageController : ApiController
    {
        [HttpPost]
        [Route("AddItemImages")]
        public bool AddItemImages(ItemImageDto dataDto)
        {
            if (dataDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (dataDto.Id > 0)
                    {
                        var data = context.ItemImages.FirstOrDefault(x => x.Id == dataDto.Id);
                        if (data != null)
                        {
                           
                            data.Name = dataDto.Name;
                            
                            data.ItemId = dataDto.ItemId;
                  
                            //data.Image = itemDto.Image;


                            context.Entry(data).Property(x => x.Name).IsModified = true;
                            context.Entry(data).Property(x => x.ItemId).IsModified = true;

                            //context.Entry(data).Property(x => x.Image).IsModified = true;


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
                                data.Image = string.Concat("UploadedFiles\\" + id + ".jpg");
                            }


                            context.SaveChanges();
                            return true;

                        }
                        return false;
                    }

                    else
                    {
                        ItemImage item = new ItemImage();

                        
                        item.Name = dataDto.Name;
                        item.ItemId = dataDto.ItemId;


                        //item.Image = itemDto.Image;
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


                        context.ItemImages.Add(item);
                        context.SaveChanges();
                        return true;
                    }

                }

            }
            return false;
        }


        [HttpGet]
        [Route("DeleteItemImages/{id}")]
        public bool DeleteItemImages(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.ItemImages.FirstOrDefault(x => x.Id == id);
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

        //[HttpPost]
        //[Route("ItemImageInView")]
        //public DataSourceResult ItemImageInView(DataSourceRequest Request)
        //{
        //    using (EAharaDB context = new EAharaDB())
        //    {
        //        var dataSourceResult = context.ItemImages.Where(x => x.IsActive == true)
        //            .Select(x => new ItemImageDto
        //            {

        //                Id = x.Id,
        //                Name = x.Name,
        //                Image = x.Image,
        //                IsActive = x.IsActive,
        //                ItemId = x.ItemId,
        //                Item = new ItemDto
        //                {
        //                    Id = x.Item.Id,
        //                    Name = x.Item.Name ,
        //                },
        //            }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

        //        DataSourceResult kendoResponseDto = new DataSourceResult();
        //        kendoResponseDto.Data = dataSourceResult.Data;
        //        kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
        //        kendoResponseDto.Total = dataSourceResult.Total;
        //        return kendoResponseDto;
        //    }
        //}

        [HttpGet]
        [Route("GetItemImage/{id}")]
        public List<ItemImageDto> GetItemImage(long id)
        {
            if (id > 0)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    var data = context.ItemImages.Where(x => x.IsActive && x.ItemId == id)
                        .Select(x => new ItemImageDto
                        {
                            Id = x.Id,
                            ItemId = x.ItemId,
                            Name = x.Name,
                            Image = x.Image,

                            Item = new ItemDto
                            {

                                Name = x.Item.Name,
                                Id = x.Item.Id,
                            }

                        }).OrderByDescending(x => x.Id).ToList();

                    return data;
                }
            }
            return null;
        }
    }
}
