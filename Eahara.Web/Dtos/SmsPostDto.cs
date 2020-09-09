using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class SmsPostDto
    {

        public string Message { get; set; }
        public List<string> Emails { get; set; }
        public List<string> MobileNos { get; set; }
        public bool AllCustomers { get; set; }
        public int OTP { get; set; }
        public long UserId { get; set; }
    }
}