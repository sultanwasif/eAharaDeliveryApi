using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class Feedback
    {
        public long Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(100)]
        public string Designation { get; set; }
        [StringLength(100)]
        public string PhoneNo { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public int Satisfaction { get; set; }
        public bool IsActive { get; set; }
        public bool IsAccepted { get; set; }
    }
}
