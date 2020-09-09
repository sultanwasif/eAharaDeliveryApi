using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class AddressDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }

        public long CustomerId { get; set; }
        public virtual CustomerDto Customer { get; set; }
    }
}