using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class LocationDto
    {
        public long Id { get; set; }       
        public string Name { get; set; }       
        public string Lat { get; set; }      
        public string Lng { get; set; }
        public float DeliveryRange { get; set; }
        public float DeliveryCharge { get; set; }
        public bool IsActive { get; set; }

    }
}