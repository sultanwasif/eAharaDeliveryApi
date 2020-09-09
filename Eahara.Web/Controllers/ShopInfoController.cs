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
    public class ShopInfoController : ApiController
    {
        [Route("AddShopInfo")]
        public bool AddShopInfo(ShopInfoDto dataDto)
        {
            if (dataDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    ShopInfo addData = new ShopInfo();

                    addData.Description = dataDto.Description;
                    addData.ShopId = dataDto.ShopId;
                    addData.IsActive = true;
                    context.ShopInfos.Add(addData);
                    context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        [HttpGet]
        [Route("DeleteShopInfo/{id}")]
        public bool DeleteShopInfo(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.ShopInfos.FirstOrDefault(x => x.Id == id);
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

        [HttpPost]
        [Route("ShopInfoInView")]
        public DataSourceResult ShopInfoInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.ShopInfos.Where(x => x.IsActive == true)
                    .Select(x => new ShopInfoDto
                    {

                        Id = x.Id,
                        Description = x.Description,
                        IsActive = x.IsActive,
                        ShopId = x.ShopId,
                        Shop = new ShopDto
                        {
                            Id = x.Shop.Id,
                            Name = x.Shop.Name,
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
        [Route("GetShopInfo/{id}")]
        public List<ShopInfoDto> GetShopInfo(long id)
        {
            if (id > 0)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    var data = context.ShopInfos.Where(x => x.IsActive && x.ShopId == id)
                        .Select(x => new ShopInfoDto
                        {
                            Id = x.Id,
                            Description = x.Description,
                            IsActive = x.IsActive,
                            ShopId = x.ShopId,
                            Shop = new ShopDto
                            {
                                Id = x.Shop.Id,
                                Name = x.Shop.Name,
                            },
                        }).OrderByDescending(x => x.Id).ToList();

                    return data;
                }
            }
            return null;
        }

    }
}
