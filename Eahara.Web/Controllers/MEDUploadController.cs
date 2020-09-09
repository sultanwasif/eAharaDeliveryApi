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
    public class MEDUploadController : ApiController
    {
        [HttpPost]
        [Route("AddUploadList")]
        public bool AddUploadList(MEDUploadDto itemDto)
        {
            if (itemDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    MEDUpload item = new MEDUpload();

                    item.Name = itemDto.Name;
                    item.EmailId = itemDto.EmailId;
                    item.MobileNo = itemDto.MobileNo;
                    item.OrderDate = itemDto.OrderDate;
                    item.Date = DateTime.Now;
                    item.IsActive = true;

                    if (itemDto.CustomerId > 0)
                    {
                        var customer = context.Customers.FirstOrDefault(x=>x.Id == itemDto.CustomerId);
                        item.Name = customer.Name;
                        item.EmailId = customer.Email;
                        item.MobileNo = customer.MobileNo;
                    }

                    if (itemDto.Path != null && itemDto.Path != "")
                    {
                        Guid id = Guid.NewGuid();
                        var imgData = itemDto.Path.Substring(itemDto.Path.IndexOf(",") + 1);
                        byte[] bytes = Convert.FromBase64String(imgData);
                        Image image;
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            image = Image.FromStream(ms);
                        }
                        Bitmap b = new Bitmap(image);
                        string filePath = System.Web.HttpContext.Current.Server.MapPath("~") + "UploadedFiles\\" + id + ".jpg";
                        b.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        item.Path = string.Concat("UploadedFiles\\" + id + ".jpg");
                    }

                    if (itemDto.Path2 != null && itemDto.Path2 != "")
                    {
                        Guid id = Guid.NewGuid();
                        var imgData = itemDto.Path2.Substring(itemDto.Path2.IndexOf(",") + 1);
                        byte[] bytes = Convert.FromBase64String(imgData);
                        Image image;
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            image = Image.FromStream(ms);
                        }
                        Bitmap b = new Bitmap(image);
                        string filePath = System.Web.HttpContext.Current.Server.MapPath("~") + "UploadedFiles\\" + id + ".jpg";
                        b.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        item.Path2 = string.Concat("UploadedFiles\\" + id + ".jpg");
                    }

                    if (itemDto.Path3 != null && itemDto.Path3 != "")
                    {
                        Guid id = Guid.NewGuid();
                        var imgData = itemDto.Path3.Substring(itemDto.Path3.IndexOf(",") + 1);
                        byte[] bytes = Convert.FromBase64String(imgData);
                        Image image;
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            image = Image.FromStream(ms);
                        }
                        Bitmap b = new Bitmap(image);
                        string filePath = System.Web.HttpContext.Current.Server.MapPath("~") + "UploadedFiles\\" + id + ".jpg";
                        b.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        item.Path3 = string.Concat("UploadedFiles\\" + id + ".jpg");
                    }

                    if (itemDto.Path4 != null && itemDto.Path4 != "")
                    {
                        Guid id = Guid.NewGuid();
                        var imgData = itemDto.Path4.Substring(itemDto.Path4.IndexOf(",") + 1);
                        byte[] bytes = Convert.FromBase64String(imgData);
                        Image image;
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            image = Image.FromStream(ms);
                        }
                        Bitmap b = new Bitmap(image);
                        string filePath = System.Web.HttpContext.Current.Server.MapPath("~") + "UploadedFiles\\" + id + ".jpg";
                        b.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        item.Path4 = string.Concat("UploadedFiles\\" + id + ".jpg");
                    }

                    context.MEDUploads.Add(item);
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }

        [HttpGet]
        [Route("ReorderUpload/{id}")]
        public bool ReorderUpload(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.MEDUploads.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {
                        MEDUpload item = new MEDUpload();

                        item.Name = Delete.Name;
                        item.EmailId = Delete.EmailId;
                        item.MobileNo = Delete.MobileNo;
                        item.OrderDate = Delete.OrderDate;
                        item.CustomerId = Delete.CustomerId;
                        item.Path = Delete.Path;
                        item.Date = DateTime.Now;
                        item.IsActive = true;

                        context.MEDUploads.Add(item);
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }

            }
            return false;
        }

        [HttpPost]
        [Route("MEDUploadsInView")]
        public DataSourceResult MEDUploadsInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.MEDUploads.Where(x => x.IsActive == true)
                    .Select(x => new MEDUploadDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        MobileNo = x.MobileNo,
                        EmailId = x.EmailId,
                        Path = x.Path,
                        Remarks = x.Remarks,
                        OrderDate = x.OrderDate,
                        Date = x.Date,
                        MEDBookingId = x.MEDBookingId,
                        MEDBooking = new MEDBookingDto
                        {
                            RefNo = x.MEDBooking != null ? x.MEDBooking.RefNo : "",
                        }

                    }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }

        [HttpGet]
        [Route("MyMEDUploadsInApp/{id}")]
        public List<MEDUploadDto> MyMEDUploadsInApp(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.MEDUploads.Where(x => x.IsActive == true && x.CustomerId == id)
                    .Select(x => new MEDUploadDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        MobileNo = x.MobileNo,
                        EmailId = x.EmailId,
                        Path = x.Path,
                        Remarks = x.Remarks,
                        OrderDate = x.OrderDate,
                        Date = x.Date,
                        MEDBookingId = x.MEDBookingId,
                        MEDBooking = new MEDBookingDto
                        {
                            RefNo = x.MEDBooking != null ? x.MEDBooking.RefNo : "",
                        }
                    }).OrderByDescending(x => x.Id).ToList();

                return dataSourceResult;
            }
        }

        [HttpGet]
        [Route("DeleteMEDUploadById/{id}")]
        public bool DeleteMEDUploadById(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.MEDUploads.FirstOrDefault(x => x.Id == id);
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


    }
}
