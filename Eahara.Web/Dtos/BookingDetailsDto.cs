using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class BookingDetailsDto
    {

        public long Id { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public float TotalPrice { get; set; }
        public bool IsActive { get; set; }
        public string Remarks { get; set; }
        public long BookingId { get; set; }
        public virtual BookingDto Booking { get; set; }
        public long ItemId { get; set; }
        public virtual ItemDto Item { get; set; }
        public long ShopId { get; set; }
        public virtual ShopDto Shop { get; set; }
        public long? StatusId { get; set; }
        public virtual StatusDto Status { get; set; }
        public float DiscountPrice { get; set; }
        public float DelCharge { get; set; }
    }
}