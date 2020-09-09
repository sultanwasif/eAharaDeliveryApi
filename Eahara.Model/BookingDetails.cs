using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
   public class BookingDetails
    {


        public long Id { get; set; }
        public int Quantity { get; set; }        
        public float Price { get; set; }
        public float TotalPrice { get; set; }
        public float DiscountPrice { get; set; }
        public float DelCharge { get; set; }
        public bool IsActive { get; set; }
        public string Remarks { get; set; }
        public long BookingId { get; set; }
        public virtual Booking Booking { get; set; }
        public long ItemId { get; set; }
        public virtual Item Item { get; set; }
        public long ShopId { get; set; }
        public virtual Shop Shop { get; set; }

        public long? StatusId { get; set; }
        public virtual Status Status { get; set; }
    }
}
