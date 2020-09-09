using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class UserOTP
    {
        public long Id { get; set; }
        public int OTP { get; set; }
        public DateTime Date { get; set; }
        [StringLength(50)]
        public string MobileNo { get; set; }
    }
}
