using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Eahara.Model;
using Eahara.Web.Dtos;
using Kendo.DynamicLinq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Device.Location;
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
    public class MEDShopController : ApiController
    {

        [HttpPost]
        [Route("AddMEDShops")]
        public bool AddMEDShops(MEDShopDto dataDto)
        {
            if (dataDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (dataDto.Id <= 0)
                    {
                        MEDShop addData = new MEDShop();

                        addData.Name = dataDto.Name;
                        addData.Description = dataDto.Description;
                        addData.TagLine = dataDto.TagLine;
                        addData.Address = dataDto.Address;
                        addData.Image = dataDto.Image;
                        addData.CommissionPercentage = dataDto.CommissionPercentage;
                        addData.MobileNo = dataDto.MobileNo;
                        addData.MobileNo2 = dataDto.MobileNo2;
                        addData.MobileNo3 = dataDto.MobileNo3;
                        addData.Lat = dataDto.Lat;
                        addData.Lng = dataDto.Lng;
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


                        context.MEDShops.Add(addData);
                        context.SaveChanges();

                        return true;
                    }
                    else
                    {
                        var olddata = context.MEDShops.FirstOrDefault(x => x.Id == dataDto.Id);
                        if (olddata != null)
                        {
                            olddata.Name = dataDto.Name;
                            olddata.Description = dataDto.Description;
                            olddata.TagLine = dataDto.TagLine;
                            olddata.Address = dataDto.Address;
                            olddata.CommissionPercentage = dataDto.CommissionPercentage;
                            olddata.MobileNo = dataDto.MobileNo;
                            olddata.MobileNo2 = dataDto.MobileNo2;
                            olddata.MobileNo3 = dataDto.MobileNo3;
                            olddata.Lat = dataDto.Lat;
                            olddata.Lng = dataDto.Lng;
                            olddata.IsActive = true;
                            context.Entry(olddata).Property(x => x.Name).IsModified = true;
                            context.Entry(olddata).Property(x => x.Description).IsModified = true;
                            context.Entry(olddata).Property(x => x.TagLine).IsModified = true;
                            context.Entry(olddata).Property(x => x.Address).IsModified = true;
                            context.Entry(olddata).Property(x => x.CommissionPercentage).IsModified = true;
                            context.Entry(olddata).Property(x => x.IsActive).IsModified = true;
                            context.Entry(olddata).Property(x => x.MobileNo).IsModified = true;
                            context.Entry(olddata).Property(x => x.MobileNo3).IsModified = true;
                            context.Entry(olddata).Property(x => x.MobileNo2).IsModified = true;
                            context.Entry(olddata).Property(x => x.Lat).IsModified = true;
                            context.Entry(olddata).Property(x => x.Lng).IsModified = true;


                            if (dataDto.Image != null && dataDto.Image != "" && olddata.Image != dataDto.Image && !dataDto.Image.Contains("http"))
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
                                olddata.Image = string.Concat("UploadedFiles\\" + id + ".jpg");
                                context.Entry(olddata).Property(x => x.Image).IsModified = true;
                            }


                            context.SaveChanges();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        [HttpGet]
        [Route("DeleteMEDShops/{id}")]
        public bool DeleteMEDShops(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.MEDShops.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {
                        Delete.IsActive = false;
                        context.Entry(Delete).Property(x => x.IsActive).IsModified = true;

                        var items = context.MEDItems.Where(x => x.IsActive && x.MEDShopId == id).ToList();
                        foreach (var i in items)
                        {
                            i.IsActive = false;
                            context.Entry(i).Property(x => x.IsActive).IsModified = true;
                        }
                    }
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }

        [HttpPost]
        [Route("MEDShopsInView")]
        public DataSourceResult MEDShopsInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.MEDShops.Where(x => x.IsActive == true)
                    .Select(x => new MEDShopDto
                    {

                        Id = x.Id,
                        Description = x.Description,
                        Name = x.Name,
                        MobileNo = x.MobileNo,
                        MobileNo2 = x.MobileNo2,
                        MobileNo3 = x.MobileNo3,
                        CommissionPercentage = x.CommissionPercentage,
                        Address = x.Address,
                        TagLine = x.TagLine,
                        Image = x.Image,
                        Lng = x.Lng,
                        Lat = x.Lat,
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
        [Route("MEDShopDetailById/{id}")]
        public MEDShopDto MEDShopDetailById(long id)
        {
            MEDShopDto ShopDto = new MEDShopDto();
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDShops.FirstOrDefault(x => x.IsActive == true && x.Id == id);

                ShopDto.Id = data.Id;
                ShopDto.Name = data.Name;
                ShopDto.Description = data.Description;
                ShopDto.TagLine = data.TagLine;
                ShopDto.Address = data.Address;
                ShopDto.CommissionPercentage = data.CommissionPercentage;
                ShopDto.Image = data.Image;
                ShopDto.MobileNo = data.MobileNo;
                ShopDto.MobileNo2 = data.MobileNo2;
                ShopDto.MobileNo3 = data.MobileNo3;
                ShopDto.Lat = data.Lat;
                ShopDto.Lng = data.Lng;
                ShopDto.IsActive = true;
            }
            return ShopDto;
        }

        [HttpGet]
        [Route("MEDShopsInDropdown")]
        public List<MEDShopDto> MEDShopsInDropdown()
        {
            List<MEDShopDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDShops.Where(x => x.IsActive == true)
                    .Select(x => new MEDShopDto
                    {
                        Id = x.Id,
                        IsActive = x.IsActive,
                        CommissionPercentage = x.CommissionPercentage,
                        Name = x.Name,


                    }).ToList();

                DtoList = data;
            }
            return DtoList;
        }
        
        [HttpGet]
        [Route("MEDShopsInNoUserDropdown")]
        public List<MEDShopDto> MEDShopsInNoUserDropdown()
        {
            List<MEDShopDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDShops.Where(x => x.IsActive == true && context.Users.Where(y=>y.IsActive && y.MEDShopId == x.Id).Count() == 0)
                    .Select(x => new MEDShopDto
                    {
                        Id = x.Id,
                        IsActive = x.IsActive,
                        CommissionPercentage = x.CommissionPercentage,
                        Name = x.Name,


                    }).ToList();

                DtoList = data;
            }
            return DtoList;
        }


    }
}
