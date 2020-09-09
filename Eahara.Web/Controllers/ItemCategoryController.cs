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
    public class ItemCategoryController : ApiController
    {

        [HttpPost]
        [Route("AddItemCategory")]
        public bool AddItemCategory(ItemCategoryDto itemDto)
        {
            if (itemDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (itemDto.Id > 0)
                    {
                        var data = context.ItemCategories.FirstOrDefault(x => x.Id == itemDto.Id);
                        if (data != null)
                        {
                            data.Name = itemDto.Name;
                            data.Priority = itemDto.Priority;
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
                        ItemCategory item = new ItemCategory();

                        item.Name = itemDto.Name;
                        item.Priority = itemDto.Priority;
                        item.IsActive = true;
                        context.ItemCategories.Add(item);


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
        [Route("ItemCategorysInView")]

        public DataSourceResult ItemCategorysInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.ItemCategories.Where(x => x.IsActive == true)
                    .Select(x => new ItemCategoryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Priority = x.Priority,
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
        [Route("DeleteItemCategoryById/{id}")]
        public bool DeleteItemCategoryById(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.ItemCategories.FirstOrDefault(x => x.Id == id);
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
        [Route("ItemCategoryInDropdown")]
        public List<ItemCategoryDto> ItemCategoryInDropdown()
        {
            List<ItemCategoryDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.ItemCategories.Where(x => x.IsActive == true)
                    .Select(x => new ItemCategoryDto
                    {
                        Id = x.Id,
                        IsActive = x.IsActive,
                        Priority = x.Priority,
                        Name = x.Name,
                        Image = x.Image,
                        IsChecked = false,

                    }).ToList();

                DtoList = data;
            }
            return DtoList;
        }

        [HttpGet]
        [Route("getShopItemCats/{id}")]
        public List<ItemDto> getShopItemCats(long id)
        {
            List<ItemDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var shop = context.Shops.FirstOrDefault(x => x.Id == id);
                var data = context.ItemCategories.Where(x => x.IsActive == true && context.Items.Where(y => y.ItemCategoryId == x.Id && y.IsActive && y.Shop.ShopCategoryId == shop.ShopCategoryId && y.ShopId == id).Count() > 0)
                    .Select(x => new ItemDto
                    {
                        Id = x.Id,
                        IsActive = x.IsActive,
                        Name = x.Name,
                        Image = x.Image,
                    }).ToList();

                DtoList = data;
            }
            return DtoList;
        }

        [HttpGet]
        [Route("getShopItemsForFirstLoad/{id}")]
        public List<ItemCategoryDto> getShopItemsForFirstLoad(long id)
        {
            List<ItemCategoryDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var shop = context.Shops.FirstOrDefault(x => x.Id == id);
                var data = context.ItemCategories.Where(x => x.IsActive == true && context.Items.Where(y => y.ItemCategoryId == x.Id && y.IsActive && y.Shop.ShopCategoryId == shop.ShopCategoryId && y.ShopId == id).Count() > 0)
                    .Select(x => new ItemCategoryDto
                    {
                        Id = x.Id,
                        IsActive = x.IsActive,
                        Priority = x.Priority,
                        Name = x.Name,
                        Image = x.Image,
                        Items = context.Items.Where(y=>y.IsActive && y.ShopId == id && y.ItemCategoryId == x.Id).Select(
                            y=> new ItemDto {
                                Id = y.Id,
                                IsActive = y.IsActive,
                                Name = y.Name,
                                OfferId = y.OfferId,
                                OfferPrice = y.OfferPrice,
                                Price = y.Price,
                                Image = y.Image,
                                InActive = y.InActive,
                                Preference = y.Preference,
                                Quantity = 1,
                                Offer = new OfferDto
                                {
                                    Id = y.Offer != null ? y.Offer.Id : 0,
                                    Image = y.Offer != null ? y.Offer.Image : "",
                                    Tittle = y.Offer != null ? y.Offer.Tittle : "",
                                    Percentage = y.Offer != null ? y.Offer.Percentage : 0,
                                },
                            }).OrderBy(y=>y.OfferPrice).ToList(),
                    }).ToList();

                DtoList = data;

                var dataoffer = context.Items.Where(x => x.IsActive && x.ShopId == id && x.OfferId != null)
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
                    ItemCategoryId = x.ItemCategoryId,
                    OfferId = x.OfferId,
                    OfferPrice = x.OfferPrice,
                    InActive = x.InActive,
                    Preference = x.Preference,
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

                if(dataoffer.Count() > 0)
                {
                    ItemCategoryDto temp = new ItemCategoryDto();
                    temp.Name = "Today's Offers";
                    temp.Priority = 0;
                    temp.Items = new List<ItemDto>();
                    foreach (var offer in dataoffer)
                    {
                        temp.Items.Add(offer);
                    }
                    DtoList.Add(temp);
                }


            }
            return DtoList;
        }

    }
}
