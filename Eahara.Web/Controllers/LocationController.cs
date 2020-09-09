using Eahara.Model;
using Eahara.Web.Dtos;
using Eahara.Web.Filter;
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
    public class LocationController : ApiController
    {
        [HttpPost]
        [Route("AddLocation")]
        public bool AddLocation(LocationDto locationDto)
        {
            if (locationDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (locationDto.Id > 0)
                    {
                        var data = context.Locations.FirstOrDefault(x => x.Id == locationDto.Id);
                        if (data != null)
                        {
                            data.Name = locationDto.Name;
                            data.Lat = locationDto.Lat;
                            data.Lng = locationDto.Lng;
                            data.DeliveryCharge = locationDto.DeliveryCharge;
                            data.DeliveryRange = locationDto.DeliveryRange;

                            context.Entry(data).Property(x => x.Name).IsModified = true;
                            context.Entry(data).Property(x => x.Lng).IsModified = true;
                            context.Entry(data).Property(x => x.Lat).IsModified = true;
                            context.Entry(data).Property(x => x.DeliveryRange).IsModified = true;
                            context.Entry(data).Property(x => x.DeliveryCharge).IsModified = true;

                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        Location location = new Location();

                        location.Name = locationDto.Name;
                        location.Lat = locationDto.Lat;
                        location.Lng = locationDto.Lng;
                        location.DeliveryCharge = locationDto.DeliveryCharge;
                        location.DeliveryRange = locationDto.DeliveryRange;

                        location.IsActive = true;
                        context.Locations.Add(location);
                        
                        context.SaveChanges();
                        return true;
                    }

                }

            }
            return false;
        }

        [HttpPost]
        [Route("LocationsInView")]
        public DataSourceResult LocationsInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.Locations.Where(x => x.IsActive == true)
                    .Select(x => new LocationDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Lng = x.Lng,
                        Lat = x.Lat,
                        DeliveryRange = x.DeliveryRange,
                        DeliveryCharge = x.DeliveryCharge,
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
        [Route("DeleteLocationById/{id}")]
        public bool DeleteLocationById(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.Locations.FirstOrDefault(x => x.Id == id);
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
        [Route("checkLocationActive/{id}")]
        public LocationDto checkLocationActive(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.Locations.FirstOrDefault(x => x.Id == id);
                    if (Delete.IsActive)
                    {
                        LocationDto loc = new LocationDto();
                        loc.Id = Delete.Id;
                        loc.Name = Delete.Name;
                        loc.DeliveryCharge = Delete.DeliveryCharge;
                        loc.DeliveryRange = Delete.DeliveryRange;
                        loc.Lng = Delete.Lng;
                        loc.Lat = Delete.Lat;

                        return loc;
                    }

                    return null;
                }
            }
            return null;
        }
        [HttpGet]
        [Route("LocationInDropdown")]
        public List<LocationDto> LocationInDropdown()
        {
            List<LocationDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
               // BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;
               // var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
               var query = context.Locations.Where(x => x.IsActive == true);
               //if (user.Role == "Employee")
               // {
               //     if (user.Employee != null)
               //     {
               //         query = query.Where(x => x.Id == user.Employee.LocationId);
               //     }
               // }

               var data = query
                    .Select(x => new LocationDto
                    {
                        Id = x.Id,
                        IsActive = x.IsActive,
                        Name = x.Name,
                        Lat = x.Lat,
                        Lng = x.Lng,
                        DeliveryCharge = x.DeliveryCharge,
                        DeliveryRange = x.DeliveryRange,

                    }).ToList();

                DtoList = data;
            }
            return DtoList;
        }

        [OrangeAuthorizationFilter]
        [HttpGet]
        [Route("LocationInDropdownAdmin")]
        public List<LocationDto> LocationInDropdownAdmin()
        {
            List<LocationDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;
                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var query = context.Locations.Where(x => x.IsActive == true);
                if (user.Role == "Employee")
                {
                    if (user.Employee != null)
                    {
                        query = query.Where(x => x.Id == user.Employee.LocationId);
                    }
                }

                var data = query
                     .Select(x => new LocationDto
                     {
                         Id = x.Id,
                         IsActive = x.IsActive,
                         Name = x.Name,
                         Lat = x.Lat,
                         Lng = x.Lng,
                         DeliveryCharge = x.DeliveryCharge,
                         DeliveryRange = x.DeliveryRange,

                     }).ToList();

                DtoList = data;
            }
            return DtoList;
        }
    }
}
