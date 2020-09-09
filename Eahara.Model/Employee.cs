using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class Employee
    {
        public long Id { get; set; }
        [StringLength(60)]
        public string Name { get; set; }
        [StringLength(60)]
        public string Designation { get; set; }
        [StringLength(60)]
        public string Email { get; set; }
        [StringLength(50)]
        public string MobileNo { get; set; }
        [StringLength(50)]
        public string TelephoneNo { get; set; }
        [StringLength(200)]
        public string Address { get; set; }
        [StringLength(100)]
        public string BankName { get; set; }
        [StringLength(100)]
        public string BankAccount { get; set; }
        public bool IsActive { get; set; }
        public DateTime JoiningDate { get; set; }
        public bool IsTemp { get; set; }
        public bool IsInActive { get; set; }
        public bool IsOwnEmployee { get; set; }
        public int NormalWorkingHours { get; set; }

        public long? LocationId { get; set; }
        public virtual Location Location { get; set; }
    }
}
