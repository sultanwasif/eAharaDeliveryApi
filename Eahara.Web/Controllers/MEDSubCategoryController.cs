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
    public class MEDSubCategoryController : ApiController
    {
        [HttpPost]
        [Route("AddMEDSubCategory")]
        public bool AddMEDSubCategory(MEDSubCategoryDto itemDto)
        {
            if (itemDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (itemDto.Id > 0)
                    {
                        var data = context.MEDSubCategories.FirstOrDefault(x => x.Id == itemDto.Id);
                        if (data != null)
                        {
                            data.Name = itemDto.Name;
                            data.MEDCategoryId = itemDto.MEDCategoryId;
                            // data.Image = itemDto.Image;
                            context.Entry(data).Property(x => x.Name).IsModified = true;
                            context.Entry(data).Property(x => x.MEDCategoryId).IsModified = true;
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
                        MEDSubCategory item = new MEDSubCategory();

                        item.Name = itemDto.Name;
                        item.MEDCategoryId = itemDto.MEDCategoryId;
                        item.IsActive = true;
                        context.MEDSubCategories.Add(item);


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
        [Route("MEDSubCategoriesInView")]
        public DataSourceResult MEDSubCategoriesInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.MEDSubCategories.Where(x => x.IsActive == true)
                    .Select(x => new MEDSubCategoryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        MEDCategoryId = x.MEDCategoryId,
                        MEDCategory = new MEDCategoryDto()
                        {
                            Id = x.MEDCategory.Id,
                            Name = x.MEDCategory.Name,
                        },
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
        [Route("DeleteMEDSubCategoryById/{id}")]
        public bool DeleteMEDSubCategoryById(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.MEDSubCategories.FirstOrDefault(x => x.Id == id);
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
        [Route("MEDSubCategoryInDropdown")]
        public List<MEDSubCategoryDto> MEDSubCategoryInDropdown()
        {
            List<MEDSubCategoryDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDSubCategories.Where(x => x.IsActive == true)
                    .Select(x => new MEDSubCategoryDto
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

        [HttpGet]
        [Route("GetSubCatByCatId/{id}")]
        public List<MEDSubCategoryDto> GetSubCatByCatId(long id)
        {
            List<MEDSubCategoryDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDSubCategories.Where(x => x.IsActive == true && x.MEDCategoryId == id)
                    .Select(x => new MEDSubCategoryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Image = x.Image,
                        MEDCategoryId = x.MEDCategoryId,
                        MEDCategory = new MEDCategoryDto
                        {
                            Name = x.MEDCategory.Name,
                            Id = x.MEDCategory.Id,
                        },
                    }).ToList();

                DtoList = data;
            }
            return DtoList;
        }

        [HttpGet]
        [Route("MEDSUBCategoryByCatId/{id}")]
        public List<MEDSubCategoryDto> MEDSUBCategoryByCatId(long id)
        {
            List<MEDSubCategoryDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDSubCategories.Where(x => x.IsActive == true && x.MEDCategoryId == id)
                    .Select(x => new MEDSubCategoryDto
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
