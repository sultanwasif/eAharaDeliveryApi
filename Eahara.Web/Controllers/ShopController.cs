using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Eahara.Model;
using Eahara.Web.Filter;
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
    public class ShopController : ApiController
    {
        [HttpPost]
        [Route("AddShops")]
        public bool AddShops(ShopDto dataDto)
        {
            if (dataDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (dataDto.Id <= 0)
                    {
                        Shop addData = new Shop();

                        addData.Name = dataDto.Name;
                        addData.ShopCategoryId = dataDto.ShopCategoryId;
                        addData.Description = dataDto.Description;
                        addData.TagLine = dataDto.TagLine;
                        addData.Address = dataDto.Address;
                        addData.OpeningHours = dataDto.OpeningHours;
                        addData.Image = dataDto.Image;
                        addData.AverageCost = dataDto.AverageCost;
                        addData.CommissionPercentage = dataDto.CommissionPercentage;
                        addData.AverageRating = dataDto.AverageRating;
                        addData.Cuisines = dataDto.Cuisines;
                        addData.DeliveryTime = dataDto.DeliveryTime;
                        addData.Preference = dataDto.Preference;
                        addData.DeliveryCharge = dataDto.DeliveryCharge;
                        addData.MobileNo = dataDto.MobileNo;
                        addData.MobileNo2 = dataDto.MobileNo2;
                        addData.MobileNo3 = dataDto.MobileNo3;
                        addData.StartTime = dataDto.StartTime;
                        addData.EndTime = dataDto.EndTime;
                        addData.LocationId = dataDto.LocationId;
                        addData.Lat = dataDto.Lat;
                        addData.Lng = dataDto.Lng;
                        addData.Order = dataDto.Order;
                        addData.DeliveryRange = dataDto.DeliveryRange;
                        addData.IsActive = true;
                        addData.AverageRating = "0";

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


                        context.Shops.Add(addData);
                        context.SaveChanges();

                        return true;
                    }
                    else
                    {
                        var olddata = context.Shops.FirstOrDefault(x => x.Id == dataDto.Id);
                        if (olddata != null)
                        {
                            olddata.Name = dataDto.Name;
                            olddata.ShopCategoryId = olddata.ShopCategoryId;
                            olddata.Description = dataDto.Description;
                            olddata.TagLine = dataDto.TagLine;
                            olddata.Address = dataDto.Address;
                            olddata.OpeningHours = dataDto.OpeningHours;
                            olddata.AverageCost = dataDto.AverageCost;
                            olddata.AverageRating = dataDto.AverageRating;
                            olddata.Cuisines = dataDto.Cuisines;
                            olddata.DeliveryTime = dataDto.DeliveryTime;
                            olddata.Preference = dataDto.Preference;
                            olddata.CommissionPercentage = dataDto.CommissionPercentage;
                            olddata.DeliveryCharge = dataDto.DeliveryCharge;
                            olddata.MobileNo = dataDto.MobileNo;
                            olddata.MobileNo2 = dataDto.MobileNo2;
                            olddata.MobileNo3 = dataDto.MobileNo3;
                            olddata.StartTime = dataDto.StartTime;
                            olddata.EndTime = dataDto.EndTime;
                            olddata.LocationId = dataDto.LocationId;
                            olddata.Order = dataDto.Order;
                            olddata.Lat = dataDto.Lat;
                            olddata.Lng = dataDto.Lng;
                            olddata.DeliveryRange = dataDto.DeliveryRange;
                            olddata.IsActive = true;
                            context.Entry(olddata).Property(x => x.Name).IsModified = true;
                            context.Entry(olddata).Property(x => x.Order).IsModified = true;
                            context.Entry(olddata).Property(x => x.ShopCategoryId).IsModified = true;
                            context.Entry(olddata).Property(x => x.Description).IsModified = true;
                            context.Entry(olddata).Property(x => x.TagLine).IsModified = true;
                            context.Entry(olddata).Property(x => x.Address).IsModified = true;
                            context.Entry(olddata).Property(x => x.OpeningHours).IsModified = true;
                            context.Entry(olddata).Property(x => x.AverageCost).IsModified = true;
                            context.Entry(olddata).Property(x => x.AverageRating).IsModified = true;
                            context.Entry(olddata).Property(x => x.Cuisines).IsModified = true;
                            context.Entry(olddata).Property(x => x.CommissionPercentage).IsModified = true;
                            context.Entry(olddata).Property(x => x.DeliveryTime).IsModified = true;
                            context.Entry(olddata).Property(x => x.Preference).IsModified = true;
                            context.Entry(olddata).Property(x => x.IsActive).IsModified = true;
                            context.Entry(olddata).Property(x => x.DeliveryCharge).IsModified = true;
                            context.Entry(olddata).Property(x => x.MobileNo).IsModified = true;
                            context.Entry(olddata).Property(x => x.MobileNo3).IsModified = true;
                            context.Entry(olddata).Property(x => x.MobileNo2).IsModified = true;
                            context.Entry(olddata).Property(x => x.StartTime).IsModified = true;
                            context.Entry(olddata).Property(x => x.EndTime).IsModified = true;
                            context.Entry(olddata).Property(x => x.Lat).IsModified = true;
                            context.Entry(olddata).Property(x => x.Lng).IsModified = true;
                            context.Entry(olddata).Property(x => x.DeliveryRange).IsModified = true;


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
        [Route("DeleteShops/{id}")]
        public bool DeleteShops(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.Shops.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {
                        Delete.IsActive = false;
                        context.Entry(Delete).Property(x => x.IsActive).IsModified = true;

                        var items = context.Items.Where(x => x.IsActive && x.ShopId == id).ToList();
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

        [OrangeAuthorizationFilter]
        [HttpPost]
        [Route("ShopsInView")]
        public DataSourceResult ShopsInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;
                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var query = context.Shops.Where(x => x.IsActive == true);
                if (user.Role == "Employee")
                {
                    if (user.Employee != null)
                    {
                        query = query.Where(x => x.LocationId == user.Employee.LocationId);
                    }
                }
                var dataSourceResult = query
                    .Select(x => new ShopDto
                    {

                        Id = x.Id,
                        ShopCategoryId=x.ShopCategoryId,
                        Description = x.Description,
                        Name = x.Name,
                        Order = x.Order,
                        AverageRating = x.AverageRating,
                        MobileNo = x.MobileNo,
                        MobileNo2 = x.MobileNo2,
                        MobileNo3 = x.MobileNo3,
                        CommissionPercentage = x.CommissionPercentage,
                        AverageCost = x.AverageCost,
                        Address = x.Address,
                        OpeningHours = x.OpeningHours,
                        TagLine = x.TagLine,
                        Cuisines = x.Cuisines,
                        Image = x.Image,
                        Preference = x.Preference,
                        DeliveryTime = x.DeliveryTime,
                        DeliveryCharge = x.DeliveryCharge,
                        StartTime = x.StartTime,
                        EndTime = x.EndTime,
                        Lng = x.Lng,
                        Lat = x.Lat,
                        DeliveryRange = x.DeliveryRange,
                        LocationId = x.LocationId,
                        Location = new LocationDto
                        {
                            Id = x.Location != null ? x.Location.Id : 0,
                            Name = x.Location != null ? x.Location.Name : "",
                        },
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
        [Route("ShopsById/{id}")]
        public ShopDto ShopsById(long id)
        {
            ShopDto ShopDto = new ShopDto();
            using (EAharaDB context = new EAharaDB())
            {
                var acctype = context.Shops.FirstOrDefault(x => x.IsActive == true && x.Id == id);

                if (acctype != null)
                {
                    ShopDto.Id = acctype.Id;
                    ShopDto.ShopCategoryId = acctype.ShopCategoryId;
                    ShopDto.Name = acctype.Name;
                    ShopDto.Description = acctype.Description;
                    ShopDto.TagLine = acctype.TagLine;
                    ShopDto.Address = acctype.Address;
                    ShopDto.OpeningHours = acctype.OpeningHours;
                    ShopDto.Preference = acctype.Preference;
                    ShopDto.CommissionPercentage = acctype.CommissionPercentage;
                    ShopDto.MobileNo = acctype.MobileNo;
                    ShopDto.MobileNo2 = acctype.MobileNo2;
                    ShopDto.MobileNo3 = acctype.MobileNo3;
                    ShopDto.Image = acctype.Image;
                    ShopDto.AverageCost = acctype.AverageCost;
                    ShopDto.Order = acctype.Order;
                    ShopDto.AverageRating = acctype.AverageRating;
                    ShopDto.Cuisines = acctype.Cuisines;
                    ShopDto.DeliveryTime = acctype.DeliveryTime;
                    ShopDto.IsActive = acctype.IsActive;
                    ShopDto.DeliveryCharge = acctype.DeliveryCharge;
                    ShopDto.StartTime = acctype.StartTime;
                    ShopDto.EndTime = acctype.EndTime;
                    ShopDto.Lat = acctype.Lat;
                    ShopDto.Lng = acctype.Lng;
                    ShopDto.DeliveryRange = acctype.DeliveryRange;
                    ShopDto.LocationId = acctype.LocationId;
                    ShopDto.Location = new LocationDto
                    {
                        Id = acctype.Location != null ? acctype.Location.Id : 0,
                        Name = acctype.Location != null ? acctype.Location.Name : "",
                    };
                   
                }

            }
            return ShopDto;
        }

        [HttpGet]
        [Route("ShopDetailById/{id}")]
        public ShopDto ShopDetailById(long id)
        {
            ShopDto ShopDto = new ShopDto();
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Shops.FirstOrDefault(x => x.IsActive == true && x.Id == id);

                ShopDto.Id = data.Id;
                ShopDto.ShopCategoryId = data.ShopCategoryId;
                ShopDto.Name = data.Name;
                ShopDto.Order = data.Order;
                ShopDto.Description = data.Description;
                ShopDto.TagLine = data.TagLine;
                ShopDto.Address = data.Address;
                ShopDto.OpeningHours = data.OpeningHours;
                ShopDto.CommissionPercentage = data.CommissionPercentage;
                ShopDto.Image = data.Image;
                ShopDto.AverageCost = data.AverageCost;
                ShopDto.AverageRating = data.AverageRating;
                ShopDto.Preference = data.Preference;
                ShopDto.Cuisines = data.Cuisines;
                ShopDto.DeliveryTime = data.DeliveryTime;
                ShopDto.MobileNo = data.MobileNo;
                ShopDto.MobileNo2 = data.MobileNo2;
                ShopDto.MobileNo3 = data.MobileNo3;
                ShopDto.DeliveryCharge = data.DeliveryCharge;
                ShopDto.StartTime = data.StartTime;
                ShopDto.EndTime = data.EndTime;
                ShopDto.Lat = data.Lat;
                ShopDto.Lng = data.Lng;
                ShopDto.DeliveryRange = data.DeliveryRange;
                ShopDto.IsActive = true;

                ShopDto.ShopCategory = new ShopCategoryDto()
                {
                    Name = data.ShopCategory.Name,
                    Image = data.ShopCategory.Image,
                };

                ShopDto.LocationId = data.LocationId;
                ShopDto.Location = new LocationDto
                {
                    Id = data.Location != null ? data.Location.Id : 0,
                    Name = data.Location != null ? data.Location.Name : "",
                };

                ShopDto.ShopMenus = context.ShopMenus.Where(x => x.IsActive == true && x.ShopId == ShopDto.Id)
                .Select(x => new ShopMenuDto
                {

                    Id = x.Id,
                    Tittle = x.Tittle,
                    Image = x.Image,
                }).OrderByDescending(x => x.Id).ToList();

                ShopDto.ShopInfos = context.ShopInfos.Where(x => x.IsActive == true && x.ShopId == ShopDto.Id)
               .Select(x => new ShopInfoDto
               {

                   Id = x.Id,
                   Description = x.Description,
               }).OrderByDescending(x => x.Id).ToList();

                ShopDto.ShopImages = context.ShopImages.Where(x => x.IsActive == true && x.ShopId == ShopDto.Id)
               .Select(x => new ShopImageDto
               {

                   Id = x.Id,
                   Name = x.Name,
                   Image = x.Image,
               }).OrderByDescending(x => x.Id).ToList();

             //   ShopDto.Items = context.Items.Where(x => x.IsActive == true && x.ShopId == ShopDto.Id)
             //   .Select(x => new ItemDto
             //   {
             //   Id = x.Id,
             //   Name = x.Name,
             //   TagLine = x.TagLine,
             //   Description = x.Description,
             //   Price = x.Price,
             //   Quantity = 1,
             //   Image = x.Image,
             //   ItemsCategory = new ItemCategoryDto()
             //   {
             //       Name = x.ItemsCategory.Name,
             //       Id = x.ItemsCategory.Id,
             //       Image = x.ItemsCategory.Image,
             //   }
             //}).OrderByDescending(x => x.Id).ToList();

            }
            return ShopDto;
        }

        [OrangeAuthorizationFilter]
        [HttpGet]
        [Route("ShopsInDropdown")]
        public List<ShopDto> ShopsInDropdown()
        {
            List<ShopDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;
                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var query = context.Shops.Where(x => x.IsActive == true);
                if (user.Role == "Employee")
                {
                    if (user.Employee != null)
                    {
                        query = query.Where(x => x.LocationId == user.Employee.LocationId);
                    }
                }
                var data = query
                    .Select(x => new ShopDto
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

        [OrangeAuthorizationFilter]
        [HttpGet]
        [Route("ShopsInNoUserDropdown")]
        public List<ShopDto> ShopsInNoUserDropdown()
        {
            List<ShopDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;

                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var query = context.Shops.Where(x => x.IsActive == true && context.Users.Where(y => y.IsActive == true && y.ShopId == x.Id).Count() == 0);

                if (user.Role == "Employee")
                {
                    if (user.Employee != null)
                    {
                        query = query.Where(x => x.LocationId == user.Employee.LocationId);
                    }
                }
                var data = query
                    .Select(x => new ShopDto
                    {
                        Id = x.Id,
                        IsActive = x.IsActive,
                        Name = x.Name,
                    }).ToList();
                DtoList = data;
            }
            return DtoList;
        }

        [OrangeAuthorizationFilter]
        [HttpPost]
        [Route("ShopsInMultiSelect")]
        public DataSourceResult ShopsInMultiSelect(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;

                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var shops = context.Shops.Where(x => x.IsActive);

                if (user.Role == "Employee")
                {
                    if (user.Employee != null)
                    {
                        shops = shops.Where(x => x.LocationId == user.Employee.LocationId);
                    }
                }
                var dataSourceResult = shops
                    .Select(x => new ShopDto
                    {
                        Id = x.Id,
                        IsActive = x.IsActive,
                        Name = x.Name,
                        MobileNo = x.MobileNo,
                    }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);


                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }

        [HttpPost]
        [Route("ShopsByCatId")]
        public List<ShopDto> ShopsByCatId(FilterDto datDto)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var query = context.Shops.Where(x => x.IsActive == true && x.ShopCategoryId == datDto.CategoryId);

                if (datDto.LocationId > 0)
                {
                    query = query.Where(x=> x.LocationId == datDto.LocationId);
                }

                var returnData = query
                    .Select(x => new ShopDto
                    {
                        Id = x.Id,
                        ShopCategoryId = x.ShopCategoryId,
                        Description = x.Description,
                        Name = x.Name,
                        AverageRating = x.AverageRating,
                        AverageCost = x.AverageCost,
                        Address = x.Address,
                        OpeningHours = x.OpeningHours,
                        TagLine = x.TagLine,
                        Preference = x.Preference,
                        CommissionPercentage = x.CommissionPercentage,
                        Order = x.Order,
                        MobileNo = x.MobileNo,
                        MobileNo2 = x.MobileNo2,
                        MobileNo3 = x.MobileNo3,
                        Cuisines = x.Cuisines,
                        Image = x.Image,
                        DeliveryTime = x.DeliveryTime,
                        DeliveryCharge = x.DeliveryCharge,
                        StartTime = x.StartTime,
                        EndTime = x.EndTime,
                        Lng = x.Lng,
                        Lat = x.Lat,
                        DeliveryRange = x.DeliveryRange,
                        IsActive = x.IsActive,
                        LocationId = x.LocationId,
                        Location = new LocationDto
                        {
                            Id = x.Location != null ? x.Location.Id : 0,
                            Name = x.Location != null ? x.Location.Name : "",
                        },
                    }).OrderByDescending(x => x.Id).Skip(datDto.Pagenation).Take(12).ToList();

                return returnData;
            }
        }

        [HttpGet]
        [Route("ShopsByKeyword/{key}/{locid}")]
        public List<ShopDto> ShopsByKeyword(string key, long locid)
        {
            List<ShopDto> listdata = new List<ShopDto>();

            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Shops.Where(x => x.IsActive && x.LocationId == locid && x.Name.Contains(key) || context.Tags.Where(y=>y.IsActive && y.ShopId ==x.Id && y.Description.Contains(key)).Count()>0)
                .Select(x => new ShopDto
                {
                    Id = x.Id,
                    ShopCategoryId = x.ShopCategoryId,
                    Description = x.Description,
                    Name = x.Name,
                    AverageRating = x.AverageRating,
                    Order = x.Order,
                    AverageCost = x.AverageCost,
                    MobileNo = x.MobileNo,
                    MobileNo2 = x.MobileNo2,
                    MobileNo3 = x.MobileNo3,
                    Address = x.Address,
                    CommissionPercentage = x.CommissionPercentage,
                    OpeningHours = x.OpeningHours,
                    TagLine = x.TagLine,
                    Cuisines = x.Cuisines,
                    Preference = x.Preference,
                    Image = x.Image,
                    DeliveryTime = x.DeliveryTime,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Lng = x.Lng,
                    Lat = x.Lat,
                    DeliveryRange = x.DeliveryRange,
                    IsActive = x.IsActive,
                    DeliveryCharge = x.DeliveryCharge,
                    LocationId = x.LocationId,
                    Location = new LocationDto
                    {
                        Id = x.Location != null ? x.Location.Id : 0,
                        Name = x.Location != null ? x.Location.Name : "",
                    },
                }).OrderByDescending(x => x.Id).ToList();

                listdata = data;
            }
            return listdata;
        }

        [HttpGet]
        [Route("ShopsByKeyword/{key}")]
        public List<ShopDto> ShopsByKeyword2(string key)
        {
            List<ShopDto> listdata = new List<ShopDto>();

            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Shops.Where(x => x.IsActive && x.Name.Contains(key) || context.Tags.Where(y => y.IsActive && y.ShopId == x.Id && y.Description.Contains(key)).Count() > 0)
                .Select(x => new ShopDto
                {
                    Id = x.Id,
                    ShopCategoryId = x.ShopCategoryId,
                    Description = x.Description,
                    Name = x.Name,
                    AverageRating = x.AverageRating,
                    Order = x.Order,
                    AverageCost = x.AverageCost,
                    MobileNo = x.MobileNo,
                    MobileNo2 = x.MobileNo2,
                    MobileNo3 = x.MobileNo3,
                    Address = x.Address,
                    CommissionPercentage = x.CommissionPercentage,
                    OpeningHours = x.OpeningHours,
                    TagLine = x.TagLine,
                    Cuisines = x.Cuisines,
                    Preference = x.Preference,
                    Image = x.Image,
                    DeliveryTime = x.DeliveryTime,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Lng = x.Lng,
                    Lat = x.Lat,
                    DeliveryRange = x.DeliveryRange,
                    IsActive = x.IsActive,
                    DeliveryCharge = x.DeliveryCharge,
                    LocationId = x.LocationId,
                    Location = new LocationDto
                    {
                        Id = x.Location != null ? x.Location.Id : 0,
                        Name = x.Location != null ? x.Location.Name : "",
                    },
                }).OrderByDescending(x => x.Id).ToList();

                listdata = data;
            }
            return listdata;
        }

        [HttpGet]
        [Route("GetSimilarShops/{id}/{locid}")]
        public List<ShopDto> GetSimilarShops(long id, long locid)
        {
            List<ShopDto> listdata = new List<ShopDto>();

            using (EAharaDB context = new EAharaDB())
            {
                var shop = context.Shops.FirstOrDefault(x => x.Id == id);
                var data = context.Shops.Where(x => x.IsActive && x.Id != id && x.ShopCategoryId ==shop.ShopCategoryId && x.LocationId == locid)
                .Select(x => new ShopDto
                {
                    Id = x.Id,
                    ShopCategoryId = x.ShopCategoryId,
                    Description = x.Description,
                    Name = x.Name,
                    AverageRating = x.AverageRating,
                    AverageCost = x.AverageCost,
                    Address = x.Address,
                    CommissionPercentage = x.CommissionPercentage,
                    MobileNo = x.MobileNo,
                    MobileNo2 = x.MobileNo2,
                    MobileNo3 = x.MobileNo3,
                    OpeningHours = x.OpeningHours,
                    Order = x.Order,
                    TagLine = x.TagLine,
                    Cuisines = x.Cuisines,
                    Preference = x.Preference,
                    Image = x.Image,
                    DeliveryTime = x.DeliveryTime,
                    IsActive = x.IsActive,
                    DeliveryCharge = x.DeliveryCharge,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Lng = x.Lng,
                    Lat = x.Lat,
                    DeliveryRange = x.DeliveryRange,
                    LocationId = x.LocationId,
                    Location = new LocationDto
                    {
                        Id = x.Location != null ? x.Location.Id : 0,
                        Name = x.Location != null ? x.Location.Name : "",
                    },
                }).OrderByDescending(x => x.Id).Take(6).ToList();

                listdata = data;
            }
            return listdata;
        }

        [HttpGet]
        [Route("GetSimilarShops/{id}")]
        public List<ShopDto> GetSimilarShops2(long id)
        {
            List<ShopDto> listdata = new List<ShopDto>();

            using (EAharaDB context = new EAharaDB())
            {
                var shop = context.Shops.FirstOrDefault(x => x.Id == id);
                var data = context.Shops.Where(x => x.IsActive && x.Id != id && x.ShopCategoryId == shop.ShopCategoryId)
                .Select(x => new ShopDto
                {
                    Id = x.Id,
                    ShopCategoryId = x.ShopCategoryId,
                    Description = x.Description,
                    Name = x.Name,
                    AverageRating = x.AverageRating,
                    AverageCost = x.AverageCost,
                    Address = x.Address,
                    CommissionPercentage = x.CommissionPercentage,
                    MobileNo = x.MobileNo,
                    MobileNo2 = x.MobileNo2,
                    MobileNo3 = x.MobileNo3,
                    OpeningHours = x.OpeningHours,
                    Order = x.Order,
                    TagLine = x.TagLine,
                    Cuisines = x.Cuisines,
                    Preference = x.Preference,
                    Image = x.Image,
                    DeliveryTime = x.DeliveryTime,
                    IsActive = x.IsActive,
                    DeliveryCharge = x.DeliveryCharge,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Lng = x.Lng,
                    Lat = x.Lat,
                    DeliveryRange = x.DeliveryRange,
                    LocationId = x.LocationId,
                    Location = new LocationDto
                    {
                        Id = x.Location != null ? x.Location.Id : 0,
                        Name = x.Location != null ? x.Location.Name : "",
                    },
                }).OrderByDescending(x => x.Id).Take(6).ToList();

                listdata = data;
            }
            return listdata;
        }

        [HttpPost]
        [Route("ShopsWithFilter")]
        public List<ShopDto> ShopsInHome(FilterDto datDto)
        {
            List<ShopDto> listdata = new List<ShopDto>();

            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Shops.Where(x => x.IsActive == true);

                if (datDto.Preference == true && datDto.Preference2 != true)
                {
                    data = data.Where(x => x.Preference == "Vegetarian" || x.Preference == "Veg & Non Veg");
                }

                if (datDto.Preference2 == true && datDto.Preference != true)
                {
                    data = data.Where(x => x.Preference == "Non-Vegetarian" || x.Preference == "Veg & Non Veg");
                }

                if (datDto.Keyword != null && datDto.Keyword != "")
                {
                    data = data.Where(x => x.Name.Contains(datDto.Keyword) || context.Tags.Where(y => y.IsActive && y.Description.Contains(datDto.Keyword) && y.ShopId == x.Id).Count() > 0);
                }

                if (datDto.ShopsCategories.Count() > 0)
                {
                    data = data.Where(x => datDto.ShopsCategories.Contains(x.ShopCategoryId));
                }

                if (datDto.ItemCategories.Count() > 0)
                {
                    data = data.Where(x => context.Items.Where(y=> y.ShopId == x.Id && datDto.ItemCategories.Contains(y.ItemCategoryId) && y.IsActive).Count() > 0);
                }

                if (datDto.LocationId > 0)
                {
                    data = data.Where(x => x.LocationId == datDto.LocationId);
                }

                var dataLsit = data
                .Select(x => new ShopDto
                {
                    Id = x.Id,
                    ShopCategoryId = x.ShopCategoryId,
                    Description = x.Description,
                    Name = x.Name,
                    AverageRating = x.AverageRating,
                    AverageCost = x.AverageCost,
                    Address = x.Address,
                    CommissionPercentage = x.CommissionPercentage,
                    OpeningHours = x.OpeningHours,
                    MobileNo = x.MobileNo,
                    MobileNo2 = x.MobileNo2,
                    MobileNo3 = x.MobileNo3,
                    Order = x.Order,
                    TagLine = x.TagLine,
                    Cuisines = x.Cuisines,
                    Preference = x.Preference,
                    Image = x.Image,
                    DeliveryTime = x.DeliveryTime,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Lng = x.Lng,
                    Lat = x.Lat,
                    DeliveryRange = x.DeliveryRange,
                    IsActive = x.IsActive,
                    DeliveryCharge = x.DeliveryCharge,
                    LocationId = x.LocationId,
                    Location = new LocationDto
                    {
                        Id = x.Location != null ? x.Location.Id : 0,
                        Name = x.Location != null ? x.Location.Name : "",
                    },
                }).OrderBy(x => x.Order).Skip(datDto.Pagenation).Take(20).ToList();

                listdata = dataLsit;
            }
            return listdata;
        }


        [HttpPost]
        [Route("NearShopsInHome")]
        public List<ShopDto> NearShopsInHome(FilterDto datDto)
        {
            List<ShopDto> listdata = new List<ShopDto>();

            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Shops.Where(x => x.IsActive == true);

                var eCoord = new GeoCoordinate(Convert.ToDouble(datDto.lat), Convert.ToDouble(datDto.lng));

                //if (datDto.lng != "" && datDto.lng != null && datDto.lat != "" && datDto.lat != null)
                //{
                //    var eCoord = new GeoCoordinate(Convert.ToDouble(datDto.lat), Convert.ToDouble(datDto.lng));
                //    // data = data.Where(x=>  <= x.DeliveryRange );
                //}

                var dataLsit = data.AsEnumerable().Where(x => ((new GeoCoordinate(Convert.ToDouble(x.Lat), Convert.ToDouble(x.Lng)).GetDistanceTo(eCoord)) / 1000) <= x.DeliveryRange)
                .Select(x => new ShopDto
                {
                    Id = x.Id,
                    ShopCategoryId = x.ShopCategoryId,
                    Description = x.Description,
                    Name = x.Name,
                    AverageRating = x.AverageRating,
                    AverageCost = x.AverageCost,
                    Address = x.Address,
                    OpeningHours = x.OpeningHours,
                    CommissionPercentage = x.CommissionPercentage,
                    MobileNo = x.MobileNo,
                    MobileNo2 = x.MobileNo2,
                    MobileNo3 = x.MobileNo3,
                    TagLine = x.TagLine,
                    Cuisines = x.Cuisines,
                    Preference = x.Preference,
                    Image = x.Image,
                    DeliveryTime = x.DeliveryTime,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Order = x.Order,
                    Lng = x.Lng,
                    Lat = x.Lat,
                    DeliveryRange = x.DeliveryRange,
                    IsActive = x.IsActive,
                    DeliveryCharge = x.DeliveryCharge,
                    LocationId = x.LocationId,
                    Location = new LocationDto
                    {
                        Id = x.Location != null ? x.Location.Id : 0,
                        Name = x.Location != null ? x.Location.Name : "",
                    },
                }).AsEnumerable().OrderBy(x => (new GeoCoordinate(Convert.ToDouble(x.Lat), Convert.ToDouble(x.Lng)).GetDistanceTo(eCoord)) / 1000).Take(6).ToList();

                listdata = dataLsit;
            }
            return listdata;
        }

        [HttpPost]
        [Route("NearShopsInHome2")]
        public List<ShopDto> NearShopsInHome2(FilterDto datDto)
        {
            List<ShopDto> listdata = new List<ShopDto>();

            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Shops.Where(x => x.IsActive == true);

                var eCoord = new GeoCoordinate(Convert.ToDouble(datDto.lat), Convert.ToDouble(datDto.lng));

                //if (datDto.lng != "" && datDto.lng != null && datDto.lat != "" && datDto.lat != null)
                //{
                //    var eCoord = new GeoCoordinate(Convert.ToDouble(datDto.lat), Convert.ToDouble(datDto.lng));
                //    // data = data.Where(x=>  <= x.DeliveryRange );
                //}

                var dataLsit = data.AsEnumerable().Where(x => ((new GeoCoordinate(Convert.ToDouble(x.Lat), Convert.ToDouble(x.Lng)).GetDistanceTo(eCoord)) / 1000) <= x.DeliveryRange)
                .Select(x => new ShopDto
                {
                    Id = x.Id,
                    ShopCategoryId = x.ShopCategoryId,
                    Description = x.Description,
                    Name = x.Name,
                    AverageRating = x.AverageRating,
                    AverageCost = x.AverageCost,
                    CommissionPercentage = x.CommissionPercentage,
                    Address = x.Address,
                    OpeningHours = x.OpeningHours,
                    MobileNo = x.MobileNo,
                    MobileNo2 = x.MobileNo2,
                    MobileNo3 = x.MobileNo3,
                    TagLine = x.TagLine,
                    Order = x.Order,
                    Cuisines = x.Cuisines,
                    Preference = x.Preference,
                    Image = x.Image,
                    DeliveryTime = x.DeliveryTime,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Lng = x.Lng,
                    Lat = x.Lat,
                    DeliveryRange = x.DeliveryRange,
                    IsActive = x.IsActive,
                    DeliveryCharge = x.DeliveryCharge,
                    LocationId = x.LocationId,
                    Location = new LocationDto
                    {
                        Id = x.Location != null ? x.Location.Id : 0,
                        Name = x.Location != null ? x.Location.Name : "",
                    },
                }).AsEnumerable().OrderBy(x => (new GeoCoordinate(Convert.ToDouble(x.Lat), Convert.ToDouble(x.Lng)).GetDistanceTo(eCoord)) / 1000).ToList();

                listdata = dataLsit;
            }
            return listdata;
        }

        [HttpGet]
        [Route("GetShopInCart/{id}")]
        public ShopDto GetShopInCart(long id)
        {
            ShopDto ItemDto = new ShopDto();
            using (EAharaDB context = new EAharaDB())
            {
                var acctype = context.Shops.FirstOrDefault(x => x.Id == id && x.IsActive);

                if (acctype != null)
                {
                    ItemDto.Id = acctype.Id;
                    ItemDto.Name = acctype.Name;
                    ItemDto.Description = acctype.Description;
                    ItemDto.TagLine = acctype.TagLine;
                    ItemDto.Address = acctype.Address;
                    ItemDto.StartTime = acctype.StartTime;
                    ItemDto.EndTime = acctype.EndTime;

                    return ItemDto;
                }

            }
            return null;
        }

    }
}
