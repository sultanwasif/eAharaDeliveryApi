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
    public class MMethodController : ApiController
    {
        [HttpPost]
        [Route("AddMMethod")]
        public bool AddMMethod(MMethodDto itemDto)
        {
            if (itemDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (itemDto.Id > 0)
                    {
                        var data = context.MMethods.FirstOrDefault(x => x.Id == itemDto.Id);
                        if (data != null)
                        {
                            data.Name = itemDto.Name;                           
                            context.Entry(data).Property(x => x.Name).IsModified = true;                           

                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        MMethod item = new MMethod();

                        item.Name = itemDto.Name;
                        item.IsActive = true;
                        context.MMethods.Add(item);

                        context.SaveChanges();
                        return true;
                    }
                }

            }
            return false;
        }

        [HttpPost]
        [Route("MMethodsInView")]
        public DataSourceResult MEDBrandsInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.MMethods.Where(x => x.IsActive == true)
                    .Select(x => new MMethodDto
                    {
                        Id = x.Id,
                        Name = x.Name,
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
        [Route("DeleteMMethodById/{id}")]
        public bool DeleteMMethodById(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.MMethods.FirstOrDefault(x => x.Id == id);
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
        [Route("MMethodInDropdown")]
        public List<MMethodDto> MMethodInDropdown()
        {
            List<MMethodDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MMethods.Where(x => x.IsActive == true)
                    .Select(x => new MMethodDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                    }).ToList();

                DtoList = data;
            }
            return DtoList;
        }

    }
}
