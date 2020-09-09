using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
     public class Location
    {
        public long Id { get; set; }
        [StringLength(400)]
        public string Name { get; set; }
        [StringLength(150)]
        public string Lat { get; set; }
        [StringLength(150)]
        public string Lng { get; set; }
        public float DeliveryRange { get; set; }
        public float DeliveryCharge { get; set; }
        public bool IsActive { get; set; }

    }
}
