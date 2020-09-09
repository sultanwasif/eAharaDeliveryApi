using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class Customer
    {

        public long Id { get; set; }
        [StringLength(60)]
        public string Name { get; set; }
        [StringLength(60)]
        public string Email { get; set; }
        [StringLength(50)]
        public string MobileNo { get; set; }
        [StringLength(50)]
        public string TelephoneNo { get; set; }
        public bool IsActive { get; set; }
        [StringLength(60)]
        public string Location { get; set; }
        [StringLength(250)]
        public string Address { get; set; }
        [StringLength(250)]
        public string Photo { get; set; }
        [StringLength(100)]
        public string RefNo { get; set; }
        [StringLength(100)]
        public string InstRefNo { get; set; }
        public float Points { get; set; }
        public int BLCount { get; set; }
        public DateTime CreatedDate { get; set; }



    }
}
