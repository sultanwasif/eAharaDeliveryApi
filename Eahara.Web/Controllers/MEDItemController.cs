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
    public class MEDItemController : ApiController
    {
        [HttpPost]
        [Route("AddMEDItems")]
        public bool AddMEDItems(MEDItemDto itemDto)
        {
            if (itemDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (itemDto.Id > 0)
                    {
                        var data = context.MEDItems.FirstOrDefault(x => x.Id == itemDto.Id);
                        if (data != null)
                        {
                            data.Name = itemDto.Name;
                            data.Tagline = itemDto.Tagline;
                            data.Price = itemDto.Price;
                            data.MEDOfferId = itemDto.MEDOfferId;
                            data.MEDSubCategoryId = itemDto.MEDSubCategoryId;
                            data.IsAvailable = itemDto.IsAvailable;
                            data.OfferPrice = itemDto.Price;
                            data.Description = itemDto.Description;
                            data.MEDCategoryId = itemDto.MEDCategoryId;
                            data.MEDBrandId = itemDto.MEDBrandId;
                            data.MEDShopId = itemDto.MEDShopId;
                            data.Bookings = itemDto.Bookings;
                            if (data.MEDOfferId > 0)
                            {
                                var offer = context.MEDOffers.FirstOrDefault(x => x.Id == data.MEDOfferId);

                                if (offer.IsPercentage)
                                {
                                    data.OfferPrice = data.OfferPrice - (data.OfferPrice * offer.Percentage) / 100;
                                }
                                else
                                {
                                    data.OfferPrice = data.OfferPrice - offer.Percentage;
                                }

                            }

                            context.Entry(data).Property(x => x.Name).IsModified = true;
                            context.Entry(data).Property(x => x.MEDShopId).IsModified = true;
                            context.Entry(data).Property(x => x.Tagline).IsModified = true;
                            context.Entry(data).Property(x => x.Price).IsModified = true;
                            context.Entry(data).Property(x => x.MEDOfferId).IsModified = true;
                            context.Entry(data).Property(x => x.MEDSubCategoryId).IsModified = true;
                            context.Entry(data).Property(x => x.IsAvailable).IsModified = true;
                            context.Entry(data).Property(x => x.OfferPrice).IsModified = true;
                            context.Entry(data).Property(x => x.Description).IsModified = true;
                            context.Entry(data).Property(x => x.MEDCategoryId).IsModified = true;
                            context.Entry(data).Property(x => x.MEDBrandId).IsModified = true;
                            context.Entry(data).Property(x => x.Bookings).IsModified = true;

                            if (itemDto.Image1 != null && itemDto.Image1 != "" && data.Image1 != itemDto.Image1 && !itemDto.Image1.Contains("localhost") && !itemDto.Image1.Contains("http"))
                            {
                                data.Image1 = storeImage(itemDto.Image1);
                                context.Entry(data).Property(x => x.Image1).IsModified = true;
                            }

                            if (itemDto.Image2 != null && itemDto.Image2 != "" && data.Image2 != itemDto.Image2 && !itemDto.Image2.Contains("localhost:") && !itemDto.Image1.Contains("http"))
                            {
                                data.Image2 = storeImage(itemDto.Image2);
                                context.Entry(data).Property(x => x.Image2).IsModified = true;
                            }

                            if (itemDto.Image3 != null && itemDto.Image3 != "" && data.Image3 != itemDto.Image3 && !itemDto.Image3.Contains("localhost:") && !itemDto.Image1.Contains("http"))
                            {
                                data.Image3 = storeImage(itemDto.Image3);
                                context.Entry(data).Property(x => x.Image3).IsModified = true;
                            }
                            if (itemDto.Image4 != null && itemDto.Image4 != "" && data.Image4 != itemDto.Image4 && !itemDto.Image4.Contains("localhost:") && !itemDto.Image1.Contains("http"))
                            {
                                data.Image4 = storeImage(itemDto.Image4);
                                context.Entry(data).Property(x => x.Image4).IsModified = true;
                            }
                            context.SaveChanges();
                            return true;

                        }
                        return false;
                    }
                    else
                    {
                        MEDItem item = new MEDItem();
                        
                        item.Name = itemDto.Name;
                        item.Tagline = itemDto.Tagline;
                        item.Price = itemDto.Price;
                        item.MEDOfferId = itemDto.MEDOfferId;
                        item.MEDSubCategoryId = itemDto.MEDSubCategoryId;
                        item.IsAvailable = itemDto.IsAvailable;
                        item.MEDBrandId = itemDto.MEDBrandId;
                        item.OfferPrice = itemDto.Price;
                        item.Description = itemDto.Description;
                        item.MEDCategoryId = itemDto.MEDCategoryId;
                        item.MEDShopId = itemDto.MEDShopId;
                        item.IsActive = true;

                        if (item.MEDOfferId > 0)
                        {
                            var offer = context.MEDOffers.FirstOrDefault(x => x.Id == item.MEDOfferId);

                            if (offer.IsPercentage)
                            {
                                item.OfferPrice = item.OfferPrice - (item.OfferPrice * offer.Percentage) / 100;
                            }
                            else
                            {
                                item.OfferPrice = item.OfferPrice - offer.Percentage;
                            }

                        }

                        if (itemDto.Image1 != null && itemDto.Image1 != "")
                        {
                            item.Image1 = storeImage(itemDto.Image1);
                        }
                        if (itemDto.Image2 != null && itemDto.Image2 != "")
                        {
                            item.Image2 = storeImage(itemDto.Image2);
                        }

                        if (itemDto.Image3 != null && itemDto.Image3 != "")
                        {
                            item.Image3 = storeImage(itemDto.Image3);
                        }

                        if (itemDto.Image4 != null && itemDto.Image4 != "")
                        {
                            item.Image4 = storeImage(itemDto.Image4);
                        }
                        context.MEDItems.Add(item);
                        context.SaveChanges();
                        return true;
                    }

                }

            }
            return false;
        }

        public string storeImage(string imgdata)
        {
            Guid id = Guid.NewGuid();
            var imgData = imgdata.Substring(imgdata.IndexOf(",") + 1);
            byte[] bytes = Convert.FromBase64String(imgData);
            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }
            Bitmap b = new Bitmap(image);
            string filePath = System.Web.HttpContext.Current.Server.MapPath("~") + "UploadedFiles\\" + id + ".jpg";
            b.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
            return string.Concat("UploadedFiles\\" + id + ".jpg");
        }


        [HttpGet]
        [Route("DeleteMEDItemById/{id}")]
        public bool DeleteMEDItemById(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.MEDItems.FirstOrDefault(x => x.Id == id);
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
        [Route("RemoveOfferMedItem/{id}")]
        public bool RemoveOfferMedItem(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.MEDItems.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {
                        Delete.OfferPrice = Delete.Price;
                        Delete.MEDOfferId = null;
                        context.Entry(Delete).Property(x => x.MEDOfferId).IsModified = true;
                        context.Entry(Delete).Property(x => x.OfferPrice).IsModified = true;

                    }
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }


        [HttpGet]
        [Route("MakeActiveInactiveMedItem/{id}")]
        public bool MakeActiveInactiveMedItem(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.MEDItems.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {
                        if (Delete.IsAvailable == true)
                        {
                            Delete.IsAvailable = false;
                            context.Entry(Delete).Property(x => x.IsAvailable).IsModified = true;
                        }
                        else
                        {
                            Delete.IsAvailable = true;
                            context.Entry(Delete).Property(x => x.IsAvailable).IsModified = true;
                        }

                    }
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }

        [HttpGet]
        [Route("MEDItemsById/{id}")]
        public MEDItemDto MEDItemsById(long id)
        {
            MEDItemDto ItemDto = new MEDItemDto();
            using (EAharaDB context = new EAharaDB())
            {
                var acctype = context.MEDItems.FirstOrDefault(x => x.IsActive == true && x.Id == id);

                if (acctype != null)
                {
                    ItemDto.Id = acctype.Id;
                    ItemDto.MEDOfferId = acctype.MEDOfferId;
                    ItemDto.Name = acctype.Name;
                    ItemDto.MEDSubCategoryId = acctype.MEDSubCategoryId;
                    ItemDto.Image1 = acctype.Image1;
                    ItemDto.OfferPrice = acctype.OfferPrice;
                    ItemDto.Price = acctype.Price;
                    ItemDto.Image2 = acctype.Image2;
                    ItemDto.Description = acctype.Description;
                    ItemDto.Tagline = acctype.Tagline;
                    ItemDto.Image3 = acctype.Image3;
                    ItemDto.IsAvailable = acctype.IsAvailable;
                    ItemDto.Image4 = acctype.Image4;
                    ItemDto.IsActive = true;
                    ItemDto.MEDCategoryId = acctype.MEDCategoryId;
                    ItemDto.MEDBrandId = acctype.MEDBrandId;
                    ItemDto.MEDShopId = acctype.MEDShopId;
                    ItemDto.MEDBrand = new MEDBrandDto
                    {
                        Id = acctype.MEDBrand != null ? acctype.MEDBrand.Id : 0,
                        Name = acctype.MEDBrand != null ? acctype.MEDBrand.Name : "",
                    };
                    ItemDto.MEDShop = new MEDShopDto
                    {
                        Id = acctype.MEDShop != null ? acctype.MEDShop.Id : 0,
                        Name = acctype.MEDShop != null ? acctype.MEDShop.Name : "",
                    };

                    ItemDto.MEDOffer = new MEDOfferDto
                    {
                        Id = acctype.MEDOffer != null ?  acctype.MEDOffer.Id :0,
                        Image = acctype.MEDOffer != null ?  acctype.MEDOffer.Image : "",
                        Title = acctype.MEDOffer != null ? acctype.MEDOffer.Title : "",
                        Percentage = acctype.MEDOffer != null ?  acctype.MEDOffer.Percentage : 0,
                    };

                    ItemDto.MEDSubCategory = new MEDSubCategoryDto
                    {
                        Id = acctype.MEDSubCategory.Id,
                        Name = acctype.MEDSubCategory.Name,
                    };


                }

            }
            return ItemDto;
        }

        [HttpGet]
        [Route("GetMEDItemInCart/{id}")]
        public MEDItemDto GetMEDItemInCart(long id)
        {
            MEDItemDto ItemDto = new MEDItemDto();
            using (EAharaDB context = new EAharaDB())
            {
                var acctype = context.MEDItems.FirstOrDefault(x => x.IsActive == true && x.Id == id);

                if (acctype != null)
                {
                    ItemDto.Id = acctype.Id;
                    ItemDto.MEDOfferId = acctype.MEDOfferId;
                    ItemDto.Name = acctype.Name;
                    ItemDto.MEDSubCategoryId = acctype.MEDSubCategoryId;
                    ItemDto.OfferPrice = acctype.OfferPrice;
                    ItemDto.Price = acctype.Price;
                    ItemDto.Description = acctype.Description;
                    ItemDto.Tagline = acctype.Tagline;
                    ItemDto.IsAvailable = acctype.IsAvailable;
                    ItemDto.IsActive = true;
                }

            }
            return ItemDto;
        }

        [HttpPost]
        [Route("MEDItemsInView")]
        public DataSourceResult MEDItemsInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.MEDItems.Where(x => x.IsActive == true )
                    .Select(x => new MEDItemDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Image1 = x.Image1,
                        Tagline = x.Tagline,
                        Description = x.Description,
                        Price = x.Price,
                        Image2 = x.Image2,
                        Image3 = x.Image3,
                        Image4 = x.Image4,
                        MEDOfferId = x.MEDOfferId,
                        IsAvailable = x.IsAvailable,
                        OfferPrice = x.OfferPrice,
                        MEDCategoryId = x.MEDCategoryId,
                        MEDSubCategoryId = x.MEDSubCategoryId,                     
                        MEDBrandId = x.MEDBrandId,
                        MEDBrand = new MEDBrandDto
                        {
                            Id = x.MEDBrand != null ? x.MEDBrand.Id : 0,
                            Name = x.MEDBrand != null ? x.MEDBrand.Name : "",
                        },
                        MEDShopId = x.MEDShopId,
                        MEDShop = new MEDShopDto
                        {
                            Id = x.MEDShop != null ? x.MEDShop.Id : 0,
                            Name = x.MEDShop != null ? x.MEDShop.Name : "",
                        },
                        MEDCategory = new MEDCategoryDto
                        {
                            Id = x.MEDCategory != null ? x.MEDCategory.Id : 0,
                            Name = x.MEDCategory != null ? x.MEDCategory.Name : "",
                        },
                        MEDOffer = new MEDOfferDto
                        {
                            Id = x.MEDOffer != null ? x.MEDOffer.Id : 0,
                            Title = x.MEDOffer != null ?  x.MEDOffer.Title : "",
                        },
                        MEDSubCategory = new MEDSubCategoryDto
                        {
                            Id = x.MEDSubCategory.Id,
                            Name = x.MEDSubCategory.Name,
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
        [Route("MEDItemsInHome")]
        public List<MEDItemDto> MEDItemsInHome()
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataList = context.MEDItems.Where(x => x.IsActive == true && !x.IsAvailable)
                    .Select(x => new MEDItemDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Image1 = x.Image1,
                        Tagline = x.Tagline,
                        Description = x.Description,
                        Price = x.Price,
                        Image2 = x.Image2,
                        Image3 = x.Image3,
                        Image4 = x.Image4,
                        MEDOfferId = x.MEDOfferId,
                        IsAvailable = x.IsAvailable,
                        OfferPrice = x.OfferPrice,
                        Bookings = x.Bookings,
                        MEDBrandId = x.MEDBrandId,
                        MEDShopId = x.MEDShopId,
                        MEDShop = new MEDShopDto
                        {
                            Id = x.MEDShop != null ? x.MEDShop.Id : 0,
                            Name = x.MEDShop != null ? x.MEDShop.Name : "",
                        },
                        MEDBrand = new MEDBrandDto
                        {
                            Id = x.MEDBrand != null ? x.MEDBrand.Id : 0,
                            Name = x.MEDBrand != null ? x.MEDBrand.Name : "",
                        },
                        MEDOffer = new MEDOfferDto
                        {
                            Id = x.MEDOffer != null ?  x.MEDOffer.Id :0,
                            Title = x.MEDOffer != null ?  x.MEDOffer.Title:"",
                        },
                    }).OrderByDescending(x => x.Bookings).Take(10).ToList();

                return dataList;
            }
        }
        
        [HttpGet]
        [Route("MEDItemsByOfferId/{id}")]
        public List<MEDItemDto> MEDItemsByOfferId(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataList = context.MEDItems.Where(x => x.IsActive == true  && !x.IsAvailable && x.MEDOfferId == id)
                    .Select(x => new MEDItemDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Image1 = x.Image1,
                        Tagline = x.Tagline,
                        Description = x.Description,
                        Price = x.Price,
                        Image2 = x.Image2,
                        Image3 = x.Image3,
                        Image4 = x.Image4,
                        MEDOfferId = x.MEDOfferId,
                        IsAvailable = x.IsAvailable,
                        OfferPrice = x.OfferPrice,
                        Bookings = x.Bookings,
                        MEDBrandId = x.MEDBrandId,
                        MEDShopId = x.MEDShopId,
                        MEDShop = new MEDShopDto
                        {
                            Id = x.MEDShop != null ? x.MEDShop.Id : 0,
                            Name = x.MEDShop != null ? x.MEDShop.Name : "",
                        },
                        MEDBrand = new MEDBrandDto
                        {
                            Id = x.MEDBrand != null ? x.MEDBrand.Id : 0,
                            Name = x.MEDBrand != null ? x.MEDBrand.Name : "",
                        },
                        MEDOffer = new MEDOfferDto
                        {
                            Id = x.MEDOffer != null ?  x.MEDOffer.Id :0,
                            Title = x.MEDOffer != null ?  x.MEDOffer.Title : "",
                        },
                    }).OrderByDescending(x => x.Bookings).ToList();

                return dataList;
            }
        }
        
        [HttpGet]
        [Route("MEDItemsByCategoryId/{id}")]
        public List<MEDItemDto> MEDItemsByCategoryId(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataList = context.MEDItems.Where(x => x.IsActive == true  && !x.IsAvailable && x.MEDSubCategoryId == id)
                    .Select(x => new MEDItemDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Image1 = x.Image1,
                        Tagline = x.Tagline,
                        Description = x.Description,
                        Price = x.Price,
                        Image2 = x.Image2,
                        Image3 = x.Image3,
                        Image4 = x.Image4,
                        MEDOfferId = x.MEDOfferId,
                        IsAvailable = x.IsAvailable,
                        OfferPrice = x.OfferPrice,
                        Bookings = x.Bookings,
                        MEDShopId = x.MEDShopId,
                        MEDShop = new MEDShopDto
                        {
                            Id = x.MEDShop != null ? x.MEDShop.Id : 0,
                            Name = x.MEDShop != null ? x.MEDShop.Name : "",
                        },
                        MEDBrand = new MEDBrandDto
                        {
                            Id = x.MEDBrand != null ? x.MEDBrand.Id : 0,
                            Name = x.MEDBrand != null ? x.MEDBrand.Name : "",
                        },
                        MEDOffer = new MEDOfferDto
                        {
                            Id = x.MEDOffer != null ? x.MEDOffer.Id : 0,
                            Title = x.MEDOffer != null ? x.MEDOffer.Title : "",
                        },
                    }).OrderByDescending(x => x.Bookings).ToList();

                return dataList;
            }
        }

        [HttpGet]
        [Route("MEDItemsByMainCategory/{id}")]
        public List<MEDItemDto> MEDItemsByMainCategory(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataList = context.MEDItems.Where(x => x.IsActive == true  && !x.IsAvailable && x.MEDSubCategory.MEDCategoryId == id)
                    .Select(x => new MEDItemDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Image1 = x.Image1,
                        Tagline = x.Tagline,
                        Description = x.Description,
                        Price = x.Price,
                        Image2 = x.Image2,
                        Image3 = x.Image3,
                        Image4 = x.Image4,
                        MEDOfferId = x.MEDOfferId,
                        IsAvailable = x.IsAvailable,
                        OfferPrice = x.OfferPrice,
                        MEDShopId = x.MEDShopId,
                        MEDShop = new MEDShopDto
                        {
                            Id = x.MEDShop != null ? x.MEDShop.Id : 0,
                            Name = x.MEDShop != null ? x.MEDShop.Name : "",
                        },
                        Bookings = x.Bookings,
                        MEDBrand = new MEDBrandDto
                        {
                            Id = x.MEDBrand != null ? x.MEDBrand.Id : 0,
                            Name = x.MEDBrand != null ? x.MEDBrand.Name : "",
                        },
                        MEDOffer = new MEDOfferDto
                        {
                            Id = x.MEDOffer != null ? x.MEDOffer.Id : 0,
                            Title = x.MEDOffer != null ? x.MEDOffer.Title : "",
                        },
                    }).OrderByDescending(x => x.Bookings).Take(20).ToList();

                return dataList;
            }
        }

        [HttpGet]
        [Route("MEDItemsByKeyword/{id}")]
        public List<MEDItemDto> MEDItemsByKeyword(string id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataList = context.MEDItems.Where(x => x.IsActive == true && !x.IsAvailable && x.Name.Contains(id))
                    .Select(x => new MEDItemDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Image1 = x.Image1,
                        Tagline = x.Tagline,
                        Description = x.Description,
                        Price = x.Price,
                        Image2 = x.Image2,
                        Image3 = x.Image3,
                        Image4 = x.Image4,
                        MEDOfferId = x.MEDOfferId,
                        IsAvailable = x.IsAvailable,
                        OfferPrice = x.OfferPrice,
                        Bookings = x.Bookings,
                        MEDShopId = x.MEDShopId,
                        MEDShop = new MEDShopDto
                        {
                            Id = x.MEDShop != null ? x.MEDShop.Id : 0,
                            Name = x.MEDShop != null ? x.MEDShop.Name : "",
                        },
                        MEDBrand = new MEDBrandDto
                        {
                            Id = x.MEDBrand != null ? x.MEDBrand.Id : 0,
                            Name = x.MEDBrand != null ? x.MEDBrand.Name : "",
                        },
                        MEDOffer = new MEDOfferDto
                        {
                            Id = x.MEDOffer != null ? x.MEDOffer.Id : 0,
                            Title = x.MEDOffer != null ? x.MEDOffer.Title : "",
                        },
                    }).OrderByDescending(x => x.Bookings).Take(20).ToList();

                return dataList;
            }
        }

        [HttpGet]
        [Route("MEDItemsByKeywordNCatId/{key}/{id}")]
        public List<MEDItemDto> MEDItemsByKeywordNCatId(string key, long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataList = context.MEDItems.Where(x => x.IsActive == true && !x.IsAvailable && x.Name.Contains(key) && x.MEDSubCategoryId == id)
                    .Select(x => new MEDItemDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Image1 = x.Image1,
                        Tagline = x.Tagline,
                        Description = x.Description,
                        Price = x.Price,
                        Image2 = x.Image2,
                        Image3 = x.Image3,
                        Image4 = x.Image4,
                        MEDOfferId = x.MEDOfferId,
                        IsAvailable = x.IsAvailable,
                        OfferPrice = x.OfferPrice,
                        MEDShopId = x.MEDShopId,
                        MEDShop = new MEDShopDto
                        {
                            Id = x.MEDShop != null ? x.MEDShop.Id : 0,
                            Name = x.MEDShop != null ? x.MEDShop.Name : "",
                        },
                        Bookings = x.Bookings,
                        MEDBrand = new MEDBrandDto
                        {
                            Id = x.MEDBrand != null ? x.MEDBrand.Id : 0,
                            Name = x.MEDBrand != null ? x.MEDBrand.Name : "",
                        },
                        MEDOffer = new MEDOfferDto
                        {
                            Id = x.MEDOffer != null ? x.MEDOffer.Id : 0,
                            Title = x.MEDOffer != null ? x.MEDOffer.Title : "",
                        },
                    }).OrderByDescending(x => x.Bookings).Take(20).ToList();

                return dataList;
            }
        }

        [HttpPost]
        [Route("MEDItemsWithFilter")]
        public List<MEDItemDto> MEDItemsWithFilter(KendoFilterDto Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataList = context.MEDItems.Where(x => x.IsActive && !x.IsAvailable);

                if (Request.Keyword != null && Request.Keyword != "")
                {
                    dataList = dataList.Where(x=>x.Name.Contains(Request.Keyword));
                }

                if (Request.MinPrice > 0)
                {
                    dataList = dataList.Where(x => x.OfferPrice > Request.MinPrice && x.OfferPrice < Request.MaxPrice);
                }

                if (Request.MEDSubCategoryId > 0)
                {
                    dataList = dataList.Where(x => x.MEDSubCategoryId ==  Request.MEDSubCategoryId);
                }

                if (Request.MEDBrandId > 0)
                {
                    dataList = dataList.Where(x => x.MEDBrandId == Request.MEDBrandId);
                }

                var retdata = new List<MEDItemDto>();

                if (Request.SortBy == 2)
                {
                    retdata = dataList
                    .Select(x => new MEDItemDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Image1 = x.Image1,
                        Tagline = x.Tagline,
                        Description = x.Description,
                        Price = x.Price,
                        Image2 = x.Image2,
                        Image3 = x.Image3,
                        Image4 = x.Image4,
                        MEDOfferId = x.MEDOfferId,
                        IsAvailable = x.IsAvailable,
                        OfferPrice = x.OfferPrice,
                        MEDShopId = x.MEDShopId,
                        MEDShop = new MEDShopDto
                        {
                            Id = x.MEDShop != null ? x.MEDShop.Id : 0,
                            Name = x.MEDShop != null ? x.MEDShop.Name : "",
                        },
                        Bookings = x.Bookings,
                        MEDBrand = new MEDBrandDto
                        {
                            Id = x.MEDBrand != null ? x.MEDBrand.Id : 0,
                            Name = x.MEDBrand != null ? x.MEDBrand.Name : "",
                        },
                        MEDOffer = new MEDOfferDto
                        {
                            Id = x.MEDOffer != null ? x.MEDOffer.Id : 0,
                            Title = x.MEDOffer != null ? x.MEDOffer.Title : "",
                        },
                    }).OrderBy(x => x.OfferPrice).Skip(Request.Skip).Take(20).ToList();
                }else if (Request.SortBy == 1)
                {
                    retdata = dataList
                    .Select(x => new MEDItemDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Image1 = x.Image1,
                        Tagline = x.Tagline,
                        Description = x.Description,
                        Price = x.Price,
                        Image2 = x.Image2,
                        Image3 = x.Image3,
                        Image4 = x.Image4,
                        MEDOfferId = x.MEDOfferId,
                        IsAvailable = x.IsAvailable,
                        MEDShopId = x.MEDShopId,
                        MEDShop = new MEDShopDto
                        {
                            Id = x.MEDShop != null ? x.MEDShop.Id : 0,
                            Name = x.MEDShop != null ? x.MEDShop.Name : "",
                        },
                        OfferPrice = x.OfferPrice,
                        Bookings = x.Bookings,
                        MEDBrand = new MEDBrandDto
                        {
                            Id = x.MEDBrand != null ? x.MEDBrand.Id : 0,
                            Name = x.MEDBrand != null ? x.MEDBrand.Name : "",
                        },
                        MEDOffer = new MEDOfferDto
                        {
                            Id = x.MEDOffer != null ? x.MEDOffer.Id : 0,
                            Title = x.MEDOffer != null ? x.MEDOffer.Title : "",
                        },
                    }).OrderByDescending(x => x.OfferPrice).Skip(Request.Skip).Take(20).ToList();
                }
                else
                {
                    retdata = dataList
                    .Select(x => new MEDItemDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Image1 = x.Image1,
                        Tagline = x.Tagline,
                        Description = x.Description,
                        Price = x.Price,
                        Image2 = x.Image2,
                        Image3 = x.Image3,
                        Image4 = x.Image4,
                        MEDOfferId = x.MEDOfferId,
                        IsAvailable = x.IsAvailable,
                        MEDShopId = x.MEDShopId,
                        MEDShop = new MEDShopDto
                        {
                            Id = x.MEDShop != null ? x.MEDShop.Id : 0,
                            Name = x.MEDShop != null ? x.MEDShop.Name : "",
                        },
                        OfferPrice = x.OfferPrice,
                        Bookings = x.Bookings,
                        MEDBrand = new MEDBrandDto
                        {
                            Id = x.MEDBrand != null ? x.MEDBrand.Id : 0,
                            Name = x.MEDBrand != null ? x.MEDBrand.Name : "",
                        },
                        MEDOffer = new MEDOfferDto
                        {
                            Id = x.MEDOffer != null ? x.MEDOffer.Id : 0,
                            Title = x.MEDOffer != null ? x.MEDOffer.Title : "",
                        },
                    }).OrderByDescending(x => x.Bookings).Skip(Request.Skip).Take(20).ToList();
                }

                return retdata;
            }
        }

        [HttpGet]
        [Route("MEDItemsInDropdown")]
        public List<MEDItemDto> MEDItemsInDropdown()
        {
            List<MEDItemDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDItems.Where(x => x.IsActive == true)
                    .Select(x => new MEDItemDto
                    {
                        Id = x.Id,
                        IsActive = x.IsActive,
                        Name = x.Name,
                        MEDOfferId = x.MEDOfferId,
                        Price = x.Price,
                        OfferPrice = x.OfferPrice,
                        Image1 = x.Image1,
                    }).ToList();

                DtoList = data;
            }
            return DtoList;
        }

        [HttpGet]
        [Route("MEDItemsByMEDShopId/{id}")]
        public List<MEDItemDto> MEDItemsByMEDShopId(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataList = context.MEDItems.Where(x => x.IsActive == true  && x.MEDShopId == id)
                    .Select(x => new MEDItemDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Image1 = x.Image1,
                        Tagline = x.Tagline,
                        Description = x.Description,
                        Price = x.Price,
                        Image2 = x.Image2,
                        Image3 = x.Image3,
                        Image4 = x.Image4,
                        MEDOfferId = x.MEDOfferId,
                        IsAvailable = x.IsAvailable,
                        OfferPrice = x.OfferPrice,
                        Bookings = x.Bookings,
                        MEDShopId = x.MEDShopId,
                        MEDShop = new MEDShopDto
                        {
                            Id = x.MEDShop != null ? x.MEDShop.Id : 0,
                            Name = x.MEDShop != null ? x.MEDShop.Name : "",
                        },
                        MEDBrand = new MEDBrandDto
                        {
                            Id = x.MEDBrand != null ? x.MEDBrand.Id : 0,
                            Name = x.MEDBrand != null ? x.MEDBrand.Name : "",
                        },
                        MEDOffer = new MEDOfferDto
                        {
                            Id = x.MEDOffer != null ? x.MEDOffer.Id : 0,
                            Title = x.MEDOffer != null ? x.MEDOffer.Title : "",
                        },
                    }).OrderByDescending(x => x.Bookings).ToList();

                return dataList;
            }
        }

    }
}
