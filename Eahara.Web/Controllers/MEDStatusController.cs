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
    public class MEDStatusController : ApiController
    {
        [HttpPost]
        [Route("AddMEDStatus")]
        public bool AddMEDStatus(MEDStatusDto dataDto)
        {
            if (dataDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (dataDto.Id > 0)
                    {
                        var data = context.MEDStatuses.FirstOrDefault(x => x.Id == dataDto.Id);
                        if (data != null)
                        {
                            data.Name = dataDto.Name;
                            data.Description = dataDto.Description;

                            context.Entry(data).Property(x => x.Name).IsModified = true;
                            context.Entry(data).Property(x => x.Description).IsModified = true;

                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        MEDStatus data = new MEDStatus();

                        data.Name = dataDto.Name;
                        data.Description = dataDto.Description;
                        data.IsActive = true;
                        context.MEDStatuses.Add(data);
                        context.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }

        [HttpPost]
        [Route("MEDStatusInView")]
        public DataSourceResult MEDStatusInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.MEDStatuses.Where(x => x.IsActive == true)
                    .Select(x => new MEDStatusDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
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
        [Route("DeleteMEDStatusId/{id}")]
        public bool DeleteMEDStatusId(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.MEDStatuses.FirstOrDefault(x => x.Id == id);
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
        [Route("MEDStatusInDropdown")]
        public List<MEDStatusDto> MEDStatusInDropdown()
        {
            List<MEDStatusDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDStatuses.Where(x => x.IsActive == true)
                    .Select(x => new MEDStatusDto
                    {
                        Id = x.Id,
                        IsActive = x.IsActive,
                        Name = x.Name,

                    }).ToList();

                DtoList = data;
            }
            return DtoList;
        }
    }
}
