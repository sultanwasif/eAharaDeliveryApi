using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class MEDOfferDto
    {
        public long Id { get; set; }
        public float Percentage { get; set; }
        public bool IsPercentage { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }

        public long? MEDShopId { get; set; }
        public virtual MEDShopDto MEDShop { get; set; }
    }
}