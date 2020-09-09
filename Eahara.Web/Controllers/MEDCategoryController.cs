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
    public class MEDCategoryController : ApiController
    {
        [HttpPost]
        [Route("AddMEDCategory")]
        public bool AddMEDCategory(MEDCategoryDto itemDto)
        {
            if (itemDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (itemDto.Id > 0)
                    {
                        var data = context.MEDCategories.FirstOrDefault(x => x.Id == itemDto.Id);
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
                        MEDCategory item = new MEDCategory();

                        item.Name = itemDto.Name;
                        item.IsActive = true;
                       


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

                        context.MEDCategories.Add(item);
                        context.SaveChanges();
                        return true;
                    }
                }

            }
            return false;
        }

        [HttpPost]
        [Route("MEDCategoriesInView")]
        public DataSourceResult MEDCategoriesInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.MEDCategories.Where(x => x.IsActive == true)
                    .Select(x => new MEDCategoryDto
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
        [Route("DeleteMEDCategoryById/{id}")]
        public bool DeleteMEDCategoryById(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.MEDCategories.FirstOrDefault(x => x.Id == id);
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
        [Route("MEDCategoryInDropdown")]
        public List<MEDCategoryDto> MEDCategoryInDropdown()
        {
            List<MEDCategoryDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDCategories.Where(x => x.IsActive == true)
                    .Select(x => new MEDCategoryDto
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
