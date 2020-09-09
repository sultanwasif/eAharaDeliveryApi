using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class MEDUpload
    {
        public long Id { get; set; }
        [StringLength(250)]
        public string Path { get; set; }
        public string Path2 { get; set; }
        public string Path3 { get; set; }
        public string Path4 { get; set; }
        public long? CustomerId { get; set; }
        public DateTime Date { get; set; }
        [StringLength(500)]
        public string Remarks { get; set; }

        [StringLength(150)]
        public string Name { get; set; }
        [StringLength(150)]
        public string MobileNo { get; set; }
        [StringLength(150)]
        public string EmailId { get; set; }
        public DateTime OrderDate { get; set; }

        public bool IsActive { get; set; }
        public long? MEDBookingId { get; set; }

        public virtual MEDBooking MEDBooking { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
