using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class MEDBookingDetailDto
    {
        public long Id { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public float TotalPrice { get; set; }
        public float DiscountPrice { get; set; }
        public float DelCharge { get; set; }
        public bool IsActive { get; set; }
        public string Remarks { get; set; }
        public long MEDBookingId { get; set; }
        public virtual MEDBookingDto MEDBooking { get; set; }
        public long MEDItemId { get; set; }
        public virtual MEDItemDto MEDItem { get; set; }
        public long? MEDShopId { get; set; }
        public virtual MEDShopDto MEDShop { get; set; }
    }
}