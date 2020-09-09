using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
   public class CustomerMMethod
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public long MMethodId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual MMethod MMethod { get; set; }
        public bool IsActive { get; set; }
    }
}
