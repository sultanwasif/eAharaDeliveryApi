using Eahara.Model;
using Eahara.Web.Filter;
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
    public class ItemController : ApiController
    {
        [HttpPost]
        [Route("AddItems")]
        public bool AddItems(ItemDto itemDto)
        {
            if (itemDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (itemDto.Id > 0)
                    {
                        var data = context.Items.FirstOrDefault(x => x.Id == itemDto.Id);
                        if (data != null)
                        {
                            data.Description = itemDto.Description;
                            data.Name = itemDto.Name;
                            data.TagLine = itemDto.TagLine;
                            data.ShopId = itemDto.ShopId;
                            data.ItemCategoryId = itemDto.ItemCategoryId;                                                        
                            data.Price = itemDto.Price;
                            data.OfferId = itemDto.OfferId;
                            data.CommissionPercentage = itemDto.CommissionPercentage;
                            data.InActive = itemDto.InActive;
                            data.Preference = itemDto.Preference;
                            data.OfferPrice = itemDto.Price;

                            if (data.OfferId > 0)
                            {
                                var offer = context.Offers.FirstOrDefault(x => x.Id == data.OfferId);

                                if (offer.IsPercentage)
                                {
                                    data.OfferPrice  = data.OfferPrice - (data.OfferPrice * offer.Percentage) / 100;
                                }
                                else
                                {
                                    data.OfferPrice = data.OfferPrice - offer.Percentage;
                                }

                            }

                            context.Entry(data).Property(x => x.Name).IsModified = true;
                            context.Entry(data).Property(x => x.ShopId).IsModified = true;
                            context.Entry(data).Property(x => x.ItemCategoryId).IsModified = true;
                            context.Entry(data).Property(x => x.TagLine).IsModified = true;
                            context.Entry(data).Property(x => x.Price).IsModified = true;
                            context.Entry(data).Property(x => x.Preference).IsModified = true;
                            context.Entry(data).Property(x => x.CommissionPercentage).IsModified = true;
                            context.Entry(data).Property(x => x.Description).IsModified = true;
                            context.Entry(data).Property(x => x.InActive).IsModified = true;
                            context.Entry(data).Property(x => x.OfferPrice).IsModified = true;

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
                        Item item = new Item();

                        item.Description = itemDto.Description;
                        item.Name = itemDto.Name;
                        item.ShopId = itemDto.ShopId;
                        item.ItemCategoryId = itemDto.ItemCategoryId;
                        item.TagLine = itemDto.TagLine;
                        item.Price = itemDto.Price;
                        item.OfferId = itemDto.OfferId;
                        item.CommissionPercentage = itemDto.CommissionPercentage;
                        item.InActive = itemDto.InActive;
                        item.IsActive = true;
                        item.OfferPrice = itemDto.Price;
                        item.Preference = itemDto.Preference;

                        if (item.OfferId > 0)
                        {
                            var offer = context.Offers.FirstOrDefault(x => x.Id == item.OfferId);

                            if (offer.IsPercentage)
                            {
                                item.OfferPrice = item.OfferPrice  - (item.OfferPrice * offer.Percentage) / 100;
                            }
                            else
                            {
                                item.OfferPrice = item.OfferPrice - offer.Percentage;
                            }                          

                        }

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


                        context.Items.Add(item);
                        context.SaveChanges();
                        return true;
                    }     

                }
                
            }
            return false;
        }

        [HttpGet]
        [Route("DeleteItem/{id}")]
        public bool DeleteItemById(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.Items.FirstOrDefault(x => x.Id == id);
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
        [Route("makeItemInactive/{id}")]
        public bool makeItemInactive(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.Items.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {
                        if (Delete.InActive)
                        {
                            Delete.InActive = false;
                            context.Entry(Delete).Property(x => x.InActive).IsModified = true;
                        }
                        else
                        {
                            Delete.InActive = true;
                            context.Entry(Delete).Property(x => x.InActive).IsModified = true;
                        }

                    }
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }

        [HttpGet]
        [Route("RemoveItemoffer/{id}")]
        public bool RemoveItemoffer(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.Items.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {

                        Delete.OfferId = null;
                        context.Entry(Delete).Property(x => x.OfferId).IsModified = true;
                        Delete.OfferPrice = Delete.Price;
                        context.Entry(Delete).Property(x => x.OfferPrice).IsModified = true;
                    }
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }

        [OrangeAuthorizationFilter]
        [HttpPost]
        [Route("ItemsInView")]
        public DataSourceResult ItemsInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;
                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var query = context.Items.Where(x => x.IsActive == true);
                if (user.Role == "Employee")
                {
                    if (user.Employee != null)
                    {
                        query = query.Where(x => x.Shop.LocationId == user.Employee.LocationId);
                    }
                }
                var dataSourceResult = query
                    .Select(x => new ItemDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Image = x.Image,
                        TagLine = x.TagLine,
                        Description = x.Description,
                        Price = x.Price,
                        CommissionPercentage = x.CommissionPercentage,
                        ShopId = x.ShopId,
                        ItemCategoryId = x.ItemCategoryId,
                        Preference = x.Preference,
                        OfferId = x.OfferId,
                        OfferPrice = x.OfferPrice,
                        InActive = x.InActive,

                        Shop = new ShopDto
                        {
                            Name = x.Shop.Name,
                            Id = x.Shop.Id,
                            DeliveryCharge = x.Shop.DeliveryCharge,
                        },
                         ItemsCategory = new ItemCategoryDto
                         {
                             Name = x.ItemsCategory.Name,
                             Id = x.ItemsCategory.Id,
                         }


                    }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }


        [HttpPost]
        [Route("ShopItemsInView")]
        public DataSourceResult ShopItemsInView(KendoFilterDto Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.Items.Where(x => x.IsActive == true && x.ShopId == Request.ShopId)
                    .Select(x => new ItemDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Image = x.Image,
                        TagLine = x.TagLine,
                        Description = x.Description,
                        Price = x.Price,
                        CommissionPercentage = x.CommissionPercentage,
                        ShopId = x.ShopId,
                        Preference = x.Preference,
                        ItemCategoryId = x.ItemCategoryId,
                        OfferId = x.OfferId,
                        OfferPrice = x.OfferPrice,
                        InActive = x.InActive,

                        Shop = new ShopDto
                        {
                            Name = x.Shop.Name,
                            Id = x.Shop.Id,
                            DeliveryCharge = x.Shop.DeliveryCharge,
                        },
                        ItemsCategory = new ItemCategoryDto
                        {
                            Name = x.ItemsCategory.Name,
                            Id = x.ItemsCategory.Id,
                        },
                        Offer = new OfferDto
                        {
                            Id = x.Offer != null ? x.Offer.Id : 0,
                            Image = x.Offer != null ? x.Offer.Image : "",
                            Tittle = x.Offer != null ? x.Offer.Tittle : "",
                            Percentage = x.Offer != null ? x.Offer.Percentage : 0,
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
        [Route("ItemsById/{id}")]
        public ItemDto ItemsById(long id)
        {
            ItemDto ItemDto = new ItemDto();
            using (EAharaDB context = new EAharaDB())
            {
                var acctype = context.Items.FirstOrDefault(x => x.IsActive == true && x.Id == id);

                if (acctype != null)
                {
                    ItemDto.Id = acctype.Id;
                    ItemDto.ItemCategoryId = acctype.ItemCategoryId;
                    ItemDto.Name = acctype.Name;
                    ItemDto.ShopId = acctype.ShopId;
                    ItemDto.OfferId = acctype.OfferId;
                    ItemDto.OfferPrice = acctype.OfferPrice;
                    ItemDto.Price = acctype.Price;
                    ItemDto.CommissionPercentage = acctype.CommissionPercentage;
                    ItemDto.Description = acctype.Description;
                    ItemDto.TagLine = acctype.TagLine;                    
                    ItemDto.Image = acctype.Image;
                    ItemDto.InActive = acctype.InActive;
                    ItemDto.Preference = acctype.Preference;
                    ItemDto.IsActive = true;
                    ItemDto.Offer = new OfferDto
                    {
                        Id = acctype.Offer != null ? acctype.Offer.Id : 0,
                        Image = acctype.Offer != null ? acctype.Offer.Image : "",
                        Tittle = acctype.Offer != null ? acctype.Offer.Tittle : "",
                        Percentage = acctype.Offer != null ? acctype.Offer.Percentage : 0,
                    };

                }

            }
            return ItemDto;
        }

        [HttpGet]
        [Route("ItemsInDropdown")]
        public List<ItemDto> ItemsInDropdown()
        {
            List<ItemDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Items.Where(x => x.IsActive == true)
                    .Select(x => new ItemDto
                    {
                        Id = x.Id,
                        IsActive = x.IsActive,
                        Name = x.Name,
                        OfferId = x.OfferId,
                        Price = x.Price,
                        CommissionPercentage = x.CommissionPercentage,
                        Preference = x.Preference,
                        OfferPrice = x.OfferPrice,
                        Image = x.Image,
                    }).ToList();

                DtoList = data;
            }
            return DtoList;
        }

        [HttpGet]
        [Route("ItemsByCatId/{id}/{locId}")]
        public List<ItemDto> ItemsByCatId(long id, long locId)
        {
            List<ItemDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Items.Where(x => x.IsActive == true && x.ItemCategoryId == id && x.Shop.LocationId == locId)
                    .Select(x => new ItemDto
                    {
                        Id = x.Id,
                        IsActive = x.IsActive,
                        Name = x.Name,
                        OfferId = x.OfferId,
                        OfferPrice = x.OfferPrice,
                        Price = x.Price,
                        CommissionPercentage = x.CommissionPercentage,
                        Image = x.Image,
                        InActive = x.InActive,
                        Preference = x.Preference,
                        Offer = new OfferDto
                        {
                            Id = x.Offer != null ? x.Offer.Id : 0,
                            Image = x.Offer != null ? x.Offer.Image : "",
                            Tittle = x.Offer != null ? x.Offer.Tittle : "",
                            Percentage = x.Offer != null ? x.Offer.Percentage : 0,
                        },
                        Shop = new ShopDto
                        {
                            Name = x.Shop.Name,
                            Id = x.Shop.Id,
                            DeliveryCharge = x.Shop.DeliveryCharge,
                        },
                    }).ToList();

                DtoList = data;
            }
            return DtoList;
        }

        [HttpGet]
        [Route("ItemsByCatId/{id}")]
        public List<ItemDto> ItemsByCatId2(long id)
        {
            List<ItemDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Items.Where(x => x.IsActive == true && x.ItemCategoryId == id)
                    .Select(x => new ItemDto
                    {
                        Id = x.Id,
                        IsActive = x.IsActive,
                        Name = x.Name,
                        OfferId = x.OfferId,
                        OfferPrice = x.OfferPrice,
                        Price = x.Price,
                        CommissionPercentage = x.CommissionPercentage,
                        Image = x.Image,
                        InActive = x.InActive,
                        Preference = x.Preference,
                        Offer = new OfferDto
                        {
                            Id = x.Offer != null ? x.Offer.Id : 0,
                            Image = x.Offer != null ? x.Offer.Image : "",
                            Tittle = x.Offer != null ? x.Offer.Tittle : "",
                            Percentage = x.Offer != null ? x.Offer.Percentage : 0,
                        },
                        Shop = new ShopDto
                        {
                            Name = x.Shop.Name,
                            Id = x.Shop.Id,
                            DeliveryCharge = x.Shop.DeliveryCharge,
                        },
                    }).ToList();

                DtoList = data;
            }
            return DtoList;
        }

        [HttpGet]
        [Route("ItemsByOfferId/{id}")]
        public List<ItemDto> ItemsByOfferId(long id)
        {
            List<ItemDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Items.Where(x => x.IsActive == true && x.OfferId == id)
                    .Select(x => new ItemDto
                    {
                        Id = x.Id,
                        IsActive = x.IsActive,
                        Name = x.Name,
                        OfferId = x.OfferId,
                        OfferPrice = x.OfferPrice,
                        Price = x.Price,
                        CommissionPercentage = x.CommissionPercentage,
                        Image = x.Image,
                        InActive = x.InActive,
                        Preference = x.Preference,
                        Offer = new OfferDto
                        {
                            Id = x.Offer != null ? x.Offer.Id : 0,
                            Image = x.Offer != null ? x.Offer.Image : "",
                            Tittle = x.Offer != null ? x.Offer.Tittle : "",
                            Percentage = x.Offer != null ? x.Offer.Percentage : 0,
                        },
                        Shop = new ShopDto
                        {
                            Name = x.Shop.Name,
                            Id = x.Shop.Id,
                            DeliveryCharge = x.Shop.DeliveryCharge,
                        },
                    }).ToList();

                DtoList = data;
            }
            return DtoList;
        }

     
       
        [HttpGet]
        [Route("ItemDetailById/{id}")]
        public ItemDto ItemDetailById(long id)
        {
            ItemDto ItemDto = new ItemDto();
            using (EAharaDB context = new EAharaDB())
            {
                var acctype = context.Items.FirstOrDefault(x => x.IsActive == true && x.Id == id);

                if (acctype != null)
                {
                    ItemDto.Id = acctype.Id;
                    ItemDto.ItemCategoryId = acctype.ItemCategoryId;
                    ItemDto.Name = acctype.Name;
                    ItemDto.ShopId = acctype.ShopId;
                    ItemDto.Price = acctype.Price;
                    ItemDto.CommissionPercentage = acctype.CommissionPercentage;
                    ItemDto.Description = acctype.Description;
                    ItemDto.TagLine = acctype.TagLine;
                    ItemDto.Image = acctype.Image;
                    ItemDto.OfferPrice = acctype.OfferPrice;
                    ItemDto.OfferId = acctype.OfferId;
                    ItemDto.InActive = acctype.InActive;
                    ItemDto.Preference = acctype.Preference;
                    ItemDto.IsActive = true;
                    ItemDto.Offer = new OfferDto
                    {
                        Image = acctype.Offer != null ? acctype.Offer.Image : "",
                        Tittle = acctype.Offer != null ? acctype.Offer.Tittle : "",
                        Id = acctype.Offer != null ? acctype.Offer.Id : 0,
                        Percentage = acctype.Offer != null ? acctype.Offer.Percentage : 0,
                    };
                    ItemDto.ItemsCategory = new ItemCategoryDto()
                    {
                        Id = acctype.ItemsCategory.Id,
                        Name = acctype.ItemsCategory.Name,
                    };

                    ItemDto.Shop = new ShopDto()
                    {
                        Id = acctype.Shop.Id,
                        ShopCategoryId = acctype.Shop.ShopCategoryId,
                        Name = acctype.Shop.Name,
                        Description = acctype.Shop.Description,
                        TagLine = acctype.Shop.TagLine,
                        Address = acctype.Shop.Address,
                        CommissionPercentage = acctype.Shop.CommissionPercentage,
                        OpeningHours = acctype.Shop.OpeningHours,
                        Image = acctype.Shop.Image,
                        AverageCost = acctype.Shop.AverageCost,
                        AverageRating = acctype.Shop.AverageRating,
                        Cuisines = acctype.Shop.Cuisines,
                        DeliveryTime = acctype.Shop.DeliveryTime,
                        DeliveryCharge = acctype.Shop.DeliveryCharge,
                        StartTime = acctype.Shop.StartTime,
                        EndTime = acctype.Shop.EndTime,
                        DeliveryRange = acctype.Shop.DeliveryRange,

                        ShopCategory = new ShopCategoryDto()
                        {
                            Name = acctype.Shop.ShopCategory.Name,
                            Image = acctype.Shop.ShopCategory.Image,
                        },

                    };

                    ItemDto.ItemImages = context.ItemImages.Where(x => x.IsActive == true && x.ItemId == ItemDto.Id)
                       .Select(x => new ItemImageDto
                       {
                           Id = x.Id,
                           Name = x.Name,
                           Image = x.Image,
                       }).OrderByDescending(x => x.Id).ToList();

                }

            }
            return ItemDto;
        }


        [HttpGet]
        [Route("GetItemInCart/{id}")]
        public ItemDto GetItemInCart(long id)
        {
            ItemDto ItemDto = new ItemDto();
            using (EAharaDB context = new EAharaDB())
            {
                var acctype = context.Items.FirstOrDefault(x => x.IsActive == true && x.Id == id);

                if (acctype != null)
                {
                    ItemDto.Id = acctype.Id;
                    ItemDto.ItemCategoryId = acctype.ItemCategoryId;
                    ItemDto.Name = acctype.Name;
                    ItemDto.ShopId = acctype.ShopId;
                    ItemDto.Price = acctype.Price;
                    ItemDto.OfferPrice = acctype.OfferPrice;
                    ItemDto.OfferId = acctype.OfferId;
                    ItemDto.InActive = acctype.InActive;
                    ItemDto.IsActive = true;
                }

            }
            return ItemDto;
        }


        [HttpGet]
        [Route("ItemsByKeyword/{key}/{locid}")]
        public List<ItemDto> ItemsByKeyword(string key, long locid)
        {
            List<ItemDto> listdata = new List<ItemDto>();

            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Items.Where(x => x.IsActive && x.Shop.IsActive && x.Name.Contains(key) || context.Tags.Where(y => y.IsActive && y.ItemId == x.Id && y.Description.Contains(key)).Count() > 0 && x.Shop.LocationId == locid && x.Shop.IsActive)
                .Select(x => new ItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    Image = x.Image,
                    TagLine = x.TagLine,
                    Description = x.Description,
                    Price = x.Price,
                    ShopId = x.ShopId,
                    CommissionPercentage = x.CommissionPercentage,
                    OfferPrice = x.OfferPrice,
                    OfferId = x.OfferId,
                    Preference = x.Preference,
                    ItemCategoryId = x.ItemCategoryId,
                    InActive = x.InActive,
                    Offer = new OfferDto
                    {
                        Image = x.Offer != null ? x.Offer.Image : "",
                        Tittle = x.Offer != null ? x.Offer.Tittle : "",
                        Percentage = x.Offer != null ? x.Offer.Percentage : 0,
                        Id = x.Offer != null ? x.Offer.Id : 0,
                    },
                    Shop = new ShopDto
                    {
                        Name = x.Shop.Name,
                        Id = x.Shop.Id,
                        DeliveryCharge = x.Shop.DeliveryCharge,
                    },
                    ItemsCategory = new ItemCategoryDto
                    {
                        Name = x.ItemsCategory.Name,
                        Id = x.ItemsCategory.Id,
                    }
                }).OrderByDescending(x => x.Id).ToList();

                listdata = data;
            }
            return listdata;
        }
        
        [HttpGet]
        [Route("ItemsByKeyword/{key}")]
        public List<ItemDto> ItemsByKeyword2(string key)
        {
            List<ItemDto> listdata = new List<ItemDto>();

            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Items.Where(x => x.IsActive && x.Name.Contains(key) || context.Tags.Where(y => y.IsActive && y.ItemId == x.Id && y.Description.Contains(key)).Count() > 0)
                .Select(x => new ItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    Image = x.Image,
                    TagLine = x.TagLine,
                    Description = x.Description,
                    Price = x.Price,
                    ShopId = x.ShopId,
                    CommissionPercentage = x.CommissionPercentage,
                    OfferPrice = x.OfferPrice,
                    OfferId = x.OfferId,
                    Preference = x.Preference,
                    ItemCategoryId = x.ItemCategoryId,
                    InActive = x.InActive,
                    Offer = new OfferDto
                    {
                        Image = x.Offer != null ? x.Offer.Image : "",
                        Tittle = x.Offer != null ? x.Offer.Tittle : "",
                        Percentage = x.Offer != null ? x.Offer.Percentage : 0,
                        Id = x.Offer != null ? x.Offer.Id : 0,
                    },
                    Shop = new ShopDto
                    {
                        Name = x.Shop.Name,
                        Id = x.Shop.Id,
                        DeliveryCharge = x.Shop.DeliveryCharge,
                    },
                    ItemsCategory = new ItemCategoryDto
                    {
                        Name = x.ItemsCategory.Name,
                        Id = x.ItemsCategory.Id,
                    }
                }).OrderByDescending(x => x.Id).ToList();

                listdata = data;
            }
            return listdata;
        }
        

        [HttpGet]
        [Route("GetShopCatItems/{id}/{catId}")]
        public List<ItemDto> GetShopCatItems(long id, long catId)
        {
            List<ItemDto> listdata = new List<ItemDto>();

            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Items.Where(x => x.IsActive && x.ShopId == id && x.ItemCategoryId == catId)
                .Select(x => new ItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    Image = x.Image,
                    TagLine = x.TagLine,
                    Description = x.Description,
                    Price = x.Price,
                    ShopId = x.ShopId,
                    CommissionPercentage = x.CommissionPercentage,
                    ItemCategoryId = x.ItemCategoryId,
                    OfferId = x.OfferId,
                    Preference = x.Preference,
                    InActive = x.InActive,
                    OfferPrice = x.OfferPrice,
                    Quantity = 1,
                    Offer = new OfferDto
                    {
                        Image = x.Offer != null ? x.Offer.Image : "",
                        Percentage = x.Offer != null ? x.Offer.Percentage : 0,
                        Tittle = x.Offer != null ? x.Offer.Tittle : "",
                        Id = x.Offer != null ? x.Offer.Id : 0,
                    },
                    Shop = new ShopDto
                    {
                        Name = x.Shop.Name,
                        Id = x.Shop.Id,
                        DeliveryCharge = x.Shop.DeliveryCharge,
                    },
                    ItemsCategory = new ItemCategoryDto
                    {
                        Name = x.ItemsCategory.Name,
                        Id = x.ItemsCategory.Id,
                    }
                }).OrderBy(x => x.OfferPrice).ToList();

                listdata = data;
            }
            return listdata;
        }

        [HttpGet]
        [Route("GetShopItemsHavingOffer/{id}")]
        public List<ItemDto> GetShopItemsHavingOffer(long id)
        {
            List<ItemDto> listdata = new List<ItemDto>();

            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Items.Where(x => x.IsActive && x.ShopId == id && x.OfferId != null)
                .Select(x => new ItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    Image = x.Image,
                    TagLine = x.TagLine,
                    Description = x.Description,
                    Price = x.Price,
                    CommissionPercentage = x.CommissionPercentage,
                    ShopId = x.ShopId,
                    ItemCategoryId = x.ItemCategoryId,
                    OfferId = x.OfferId,
                    OfferPrice = x.OfferPrice,
                    InActive = x.InActive,
                    Preference = x.Preference,
                    Quantity = 1,
                    Offer = new OfferDto
                    {
                        Image = x.Offer!= null ? x.Offer.Image : "",
                        Percentage = x.Offer!= null ? x.Offer.Percentage : 0,
                        Tittle = x.Offer != null ? x.Offer.Tittle: "",
                        Id = x.Offer != null ? x.Offer.Id : 0,
                    },
                    Shop = new ShopDto
                    {
                        Name = x.Shop.Name,
                        Id = x.Shop.Id,
                        DeliveryCharge = x.Shop.DeliveryCharge,
                    },
                    ItemsCategory = new ItemCategoryDto
                    {
                        Name = x.ItemsCategory.Name,
                        Id = x.ItemsCategory.Id,
                    }
                }).OrderByDescending(x => x.OfferPrice).ToList();

                listdata = data;
            }
            return listdata;
        }

        [HttpGet]
        [Route("GetShopItemsByShopId/{id}")]
        public List<ItemDto> GetShopItemsByShopId(long id)
        {
            List<ItemDto> listdata = new List<ItemDto>();

            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Items.Where(x => x.IsActive && x.ShopId == id)
                .Select(x => new ItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    Image = x.Image,
                    TagLine = x.TagLine,
                    Description = x.Description,
                    Price = x.Price,
                    CommissionPercentage = x.CommissionPercentage,
                    ShopId = x.ShopId,
                    Preference = x.Preference,
                    ItemCategoryId = x.ItemCategoryId,
                    OfferId = x.OfferId,
                    InActive = x.InActive,
                    OfferPrice = x.OfferPrice,
                    Quantity = 1,
                    Offer = new OfferDto
                    {
                        Image = x.Offer!= null ? x.Offer.Image : "",
                        Percentage = x.Offer!= null ? x.Offer.Percentage : 0,
                        Tittle = x.Offer != null ? x.Offer.Tittle: "",
                        Id = x.Offer != null ? x.Offer.Id : 0,
                    },
                    Shop = new ShopDto
                    {
                        Name = x.Shop.Name,
                        Id = x.Shop.Id,
                        DeliveryCharge = x.Shop.DeliveryCharge,
                    },
                    ItemsCategory = new ItemCategoryDto
                    {
                        Name = x.ItemsCategory.Name,
                        Id = x.ItemsCategory.Id,
                    }
                }).OrderByDescending(x => x.OfferId).ToList();

                listdata = data;
            }
            return listdata;
        }

    }
}
