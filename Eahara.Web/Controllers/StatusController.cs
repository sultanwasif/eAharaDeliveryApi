﻿using Eahara.Model;
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
    public class StatusController : ApiController
    {
        [HttpPost]
        [Route("AddStatus")]
        public bool AddStatus(StatusDto locationDto)
        {
            if (locationDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (locationDto.Id > 0)
                    {
                        var data = context.Status.FirstOrDefault(x => x.Id == locationDto.Id);
                        if (data != null)
                        {
                            data.Name = locationDto.Name;

                            context.Entry(data).Property(x => x.Name).IsModified = true;

                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        Status location = new Status();

                        location.Name = locationDto.Name;

                        location.IsActive = true;
                        context.Status.Add(location);

                        context.SaveChanges();
                        return true;
                    }

                }

            }
            return false;
        }

        [HttpPost]
        [Route("StatusInView")]
        public DataSourceResult StatusInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.Status.Where(x => x.IsActive == true)
                    .Select(x => new StatusDto
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
        [Route("DeleteStatusId/{id}")]
        public bool DeleteStatusId(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.Status.FirstOrDefault(x => x.Id == id);
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
        [Route("StatusInDropdown")]
        public List<StatusDto> StatusInDropdown()
        {
            List<StatusDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Status.Where(x => x.IsActive == true)
                    .Select(x => new StatusDto
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
