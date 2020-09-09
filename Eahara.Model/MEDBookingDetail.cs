using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class MEDBookingDetail
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
        public virtual MEDBooking MEDBooking { get; set; }
        public long MEDItemId { get; set; }
        public virtual MEDItem MEDItem { get; set; }

        public long? MEDShopId { get; set; }
        public virtual MEDShop MEDShop { get; set; }
    }
}
