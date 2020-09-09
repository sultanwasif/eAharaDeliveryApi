using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
   public class MEDBooking
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsPaid { get; set; }

        [StringLength(400)]
        public string Description { get; set; }
        [StringLength(400)]
        public string Remarks { get; set; }

        [StringLength(50)]
        public string Time { get; set; }
        public DateTime Date { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? PickUpDate { get; set; }
        public DateTime? AssignedDate { get; set; }
        public DateTime? StatusDate { get; set; }
        public int Count { get; set; }
        public float Total { get; set; }
        public float PromoOfferPrice { get; set; }
        public float TotalDeliveryCharge { get; set; }
        public float SubTotal { get; set; }
        public float WalletCash { get; set; }

        [StringLength(150)]
        public string Name { get; set; }
        [StringLength(150)]
        public string MobileNo { get; set; }
        [StringLength(150)]
        public string EmailId { get; set; }
        [StringLength(250)]
        public string Address { get; set; }
        [StringLength(250)]
        public string CancelRemarks { get; set; }
        [StringLength(100)]
        public string RefNo { get; set; }

        [StringLength(200)]
        public string Lat { get; set; }
        [StringLength(200)]
        public string Lng { get; set; }

        public long? CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public long? MEDStatusId { get; set; }
        public virtual MEDStatus MEDStatus { get; set; }
        public long? PromoOfferId { get; set; }
        public virtual PromoOffer PromoOffer { get; set; }
        public long? LocationId { get; set; }
        public virtual Location Location { get; set; }
        public bool IsOrderLater { get; set; }
        public long? EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public virtual ICollection<MEDBookingDetail> MEDBookingDetails { get; set; }
    }
}
