using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class MEDUploadDto
    {
        public long Id { get; set; }
        public string Path { get; set; }
        public string Path2 { get; set; }
        public string Path3 { get; set; }
        public string Path4 { get; set; }
        public long CustomerId { get; set; }
        public DateTime Date { get; set; }
        public string Remarks { get; set; }    
        public string Name { get; set; }   
        public string MobileNo { get; set; } 
        public string EmailId { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsActive { get; set; }
        public long? MEDBookingId { get; set; }

        public virtual MEDBookingDto MEDBooking { get; set; }
        public virtual CustomerDto Customer { get; set; }
    }
}