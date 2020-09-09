using Eahara.Model;
using Eahara.Web.Dtos;
using Kendo.DynamicLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Eahara.Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TagController : ApiController
    {

        [HttpPost]
        [Route("AddTag")]
        public bool AddTag(TagDto tagdto)
        {
            if (tagdto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (tagdto.Id > 0)
                    {
                        var data = context.Tags.FirstOrDefault(x => x.Id == tagdto.Id);
                        if (data != null)
                        {
                            data.ShopId = tagdto.ShopId;
                            data.ItemId = tagdto.ItemId;
                            data.Description = tagdto.Description;

                            context.Entry(data).Property(x => x.ShopId).IsModified = true;
                            context.Entry(data).Property(x => x.ItemId).IsModified = true;
                            context.Entry(data).Property(x => x.Description).IsModified = true;


                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        Tag tag = new Tag();

                        tag.ShopId = tagdto.ShopId;
                        tag.ItemId = tagdto.ItemId;
                        tag.Description = tagdto.Description;
                        tag.IsActive = true;
                        context.Tags.Add(tag);

                        context.SaveChanges();
                        return true;
                    }

                }

            }
            return false;
        }

        [HttpPost]
        [Route("TagInView")]

        public DataSourceResult TagInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.Tags.Where(x => x.IsActive == true)
                    .Select(x => new TagDto
                    {
                        Id = x.Id,
                        ShopId = x.ShopId,
                        Description = x.Description,
                        ItemId = x.ItemId,
                        IsActive = x.IsActive,

                        Shop = new ShopDto
                        {
                            Id = x.Shop != null ? x.Shop.Id : 0,
                            Name = x.Shop != null ? x.Shop.Name : " ",
                        },

                        Item = new ItemDto
                        {
                            Id = x.Item != null ? x.Item.Id : 0,
                            Name = x.Item != null ? x.Item.Name : " ",
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
        [Route("DeleteTag/{id}")]
        public bool DeleteTag(int id)
        {
            using (EAharaDB context = new EAharaDB())
            {

                if (id != 0)
                {
                    var tag = context.Tags.FirstOrDefault(x => x.Id == id);
                    if (tag != null)
                    {
                        tag.IsActive = false;
                        context.Entry(tag).Property(x => x.IsActive).IsModified = true;

                    }
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }

        [HttpGet]
        [Route("GetShopTags/{id}")]
        public List<TagDto> GetShopTags(long id)
        {
            if (id > 0)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    var data = context.Tags.Where(x => x.IsActive && x.ShopId == id)
                        .Select(x => new TagDto
                        {
                            Id = x.Id,
                            Description = x.Description,
                            IsActive = x.IsActive,
                            ShopId = x.ShopId,
                            Shop = new ShopDto
                            {
                                Id = x.Shop != null ? x.Shop.Id : 0,
                                Name = x.Shop != null ? x.Shop.Name : " ",
                            },
                        }).OrderByDescending(x => x.Id).ToList();

                    return data;
                }
            }
            return null;
        }

        [HttpGet]
        [Route("GetItemTags/{id}")]
        public List<TagDto> GetItemTags(long id)
        {
            if (id > 0)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    var data = context.Tags.Where(x => x.IsActive && x.ItemId == id)
                        .Select(x => new TagDto
                        {
                            Id = x.Id,
                            Description = x.Description,
                            IsActive = x.IsActive,
                            ItemId = x.ItemId,
                            Item = new ItemDto
                            {
                                Id = x.Item != null ? x.Item.Id : 0,
                                Name = x.Item != null ? x.Item.Name : " ",
                            },
                        }).OrderByDescending(x => x.Id).ToList();

                    return data;
                }
            }
            return null;
        }

    }
}


