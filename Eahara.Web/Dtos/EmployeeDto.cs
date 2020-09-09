using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class EmployeeDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string TelephoneNo { get; set; }
        public string Address { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public bool IsActive { get; set; }
        public DateTime JoiningDate { get; set; }
        public bool IsTemp { get; set; }
        public bool IsInActive { get; set; }
        public bool IsOwnEmployee { get; set; }
        public int NormalWorkingHours { get; set; }

        public long? LocationId { get; set; }
        public virtual LocationDto Location { get; set; }
    }
}