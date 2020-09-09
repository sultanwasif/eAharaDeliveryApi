using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class OfferDto
    {
        public long Id { get; set; }
        public float Percentage { get; set; }
        public string Image { get; set; }
        public string Tittle { get; set; }
        public bool IsActive { get; set; }
        public bool IsPercentage { get; set; }

        public long? ShopId { get; set; }
        public virtual ShopDto Shop { get; set; }
    }
}