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
    public class AddressController : ApiController
    {
        [HttpPost]
        [Route("AddAddress")]
        public bool AddAddress(AddressDto itemDto)
        {
            if (itemDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (itemDto.Id > 0)
                    {
                        var data = context.Addresses.FirstOrDefault(x => x.Id == itemDto.Id);
                        if (data != null)
                        {
                            data.Description = itemDto.Description;
                            data.Title = itemDto.Title;
                            data.Location = itemDto.Location;
                            data.CustomerId = itemDto.CustomerId;
                            data.Lng = itemDto.Lng;
                            data.Lat = itemDto.Lat;
                            
                            context.Entry(data).Property(x => x.CustomerId).IsModified = true;
                            context.Entry(data).Property(x => x.Location).IsModified = true;
                            context.Entry(data).Property(x => x.Title).IsModified = true;
                            context.Entry(data).Property(x => x.Description).IsModified = true;
                            context.Entry(data).Property(x => x.Lat).IsModified = true;
                            context.Entry(data).Property(x => x.Lng).IsModified = true;

                            context.SaveChanges();
                            return true;

                        }
                        return false;
                    }
                    else
                    {
                        Address addresss = new Address();

                        addresss.Description = itemDto.Description;
                        addresss.Title = itemDto.Title;
                        addresss.Location = itemDto.Location;
                        addresss.CustomerId = itemDto.CustomerId;
                        addresss.Lat = itemDto.Lat;
                        addresss.Lng = itemDto.Lng;

                        context.Addresses.Add(addresss);
                        context.SaveChanges();
                        return true;
                    }

                }

            }
            return false;
        }

        [HttpGet]
        [Route("DeleteAddress/{id}")]
        public bool DeleteAddress(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.Addresses.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {
                        context.Addresses.Remove(Delete);

                    }
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }

        [HttpGet]
        [Route("AddressbyCusId/{id}")]
        public List<AddressDto> AddressbyCusId(long id)
        {
            List<AddressDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Addresses.Where(x => x.CustomerId == id)
                    .Select(x => new AddressDto
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        Location = x.Location,
                        Lat = x.Lat,
                        Lng = x.Lng,
                    }).ToList();

                DtoList = data;
            }
            return DtoList;
        }


        

    }
}
