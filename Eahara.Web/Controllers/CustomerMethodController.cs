using Eahara.Model;
using Eahara.Web.Dtos;
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
    public class CustomerMethodController : ApiController
    {
        [HttpGet]
        [Route("CustomerMMethodDetailsById/{id}")]
        public List<CustomerMMethodDto> CustomerMMethodDetailsById(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.CustomerMMethods.Where(x => x.IsActive == true && x.CustomerId == id)
                    .Select(x => new CustomerMMethodDto {
                        Id = x.Id,
                        CustomerId = x.CustomerId,
                        MMethodId = x.MMethodId,
                        MMethod = new MMethodDto
                        {
                            Id = x.MMethod.Id,
                            Name = x.MMethod.Name,
                        }
                    }).ToList();

                return data;
            }
        }

    }
}
