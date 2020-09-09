using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class CustomerDto
    {

        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string TelephoneNo { get; set; }
        public bool IsActive { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string Photo { get; set; }
        public string RefNo { get; set; }
        public string InstRefNo { get; set; }
        public float Points { get; set; }
        public int TOrders { get; set; }
        public int BLCount { get; set; }
        public DateTime CreatedDate { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }

        public virtual UserDto User { get; set; }
        public virtual List<CustomerMMethodDto> CustomerMMethods { get; set; }
    }
}