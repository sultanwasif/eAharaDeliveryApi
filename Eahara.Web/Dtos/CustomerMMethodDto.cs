using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class CustomerMMethodDto
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public long MMethodId { get; set; }

        public virtual CustomerDto Customer { get; set; }
        public virtual MMethodDto MMethod { get; set; }
        public bool IsActive { get; set; }
    }
}