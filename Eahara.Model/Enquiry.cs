using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class Enquiry
    {
        public long Id { get; set; }
        [StringLength(60)]
        public string Name { get; set; }
        [StringLength(60)]
        public string MobileNo { get; set; }
        [StringLength(60)]
        public string Email { get; set; }
        [StringLength(100)]
        public string Subject { get; set; }
        [StringLength(500)]
        public string Remarks { get; set; }

        public bool IsActive { get; set; }
        public bool IsClosed { get; set; }
    }
}
