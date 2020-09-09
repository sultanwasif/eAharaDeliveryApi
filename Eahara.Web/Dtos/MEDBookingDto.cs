using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class MEDBookingDto
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsView { get; set; }
        public bool IsPaid { get; set; }

        public string Description { get; set; }
        public string Remarks { get; set; }

        public string Time { get; set; }
        public DateTime Date { get; set; }
        public DateTime? PickUpDate { get; set; }
        public DateTime? AssignedDate { get; set; }
        public DateTime? StatusDate { get; set; }
        public int Count { get; set; }
        public float Total { get; set; }
        public float PromoOfferPrice { get; set; }
        public float TotalDeliveryCharge { get; set; }
        public float SubTotal { get; set; }
        public float WalletCash { get; set; }
        public float Commission { get; set; }
        public float ActualTotal { get; set; }
        public float Balance { get; set; }

        public string Name { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
        public string Address { get; set; }
        public string RefNo { get; set; }
        public string StatusName { get; set; }
        public string CancelRemarks { get; set; }

        public string Lat { get; set; }
        public string Lng { get; set; }

        public long AddressId { get; set; }
        public long? MEDStatusId { get; set; }
        public virtual MEDStatusDto MEDStatus { get; set; }
        public long? CustomerId { get; set; }
        public virtual CustomerDto Customer { get; set; }
        public long? LocationId { get; set; }
        public virtual LocationDto Location { get; set; }
        public bool IsOrderLater { get; set; }
        public DateTime OrderDate { get; set; }

        public long? PromoOfferId { get; set; }
        public long MEDUploadId { get; set; }
        public virtual PromoOfferDto PromoOffer { get; set; }

        public long? EmployeeId { get; set; }
        public virtual EmployeeDto Employee { get; set; }

        public virtual ICollection<MEDBookingDetailDto> MEDBookingDetails { get; set; }

        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Hour { get; set; }
        public int Minutes { get; set; }

        // counts

        public int DBooked { get; set; }
        public int DApproved { get; set; }
        public int DPicked { get; set; }
        public int DReady { get; set; }
        public int DCancelled { get; set; }
        public int DDelivered { get; set; }
        public int TAssigned { get; set; }
        public int TDelivered { get; set; }

    }
}