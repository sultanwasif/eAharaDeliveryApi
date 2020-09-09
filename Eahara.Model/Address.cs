using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class Address
    {
        public long Id { get; set; }
        [StringLength(100)]
        public string Title { get; set; }
        [StringLength(150)]
        public string Location { get; set; }
        [StringLength(400)]
        public string Description { get; set; }
        [StringLength(150)]
        public string Lat { get; set; }
        [StringLength(150)]
        public string Lng { get; set; }

        public long CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
