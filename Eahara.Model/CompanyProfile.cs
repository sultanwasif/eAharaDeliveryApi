using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class CompanyProfile
    {
        public long Id { get; set; }
        [StringLength(60)]
        public string Name { get; set; }
        [StringLength(500)]
        public string Address { get; set; }
        [StringLength(60)]
        public string Email { get; set; }
        [StringLength(60)]
        public string MobileNo { get; set; }        
        [StringLength(100)]
        public string TeleNo { get; set; }        
        [StringLength(100)]
        public string WhatsappNo { get; set; }
        [StringLength(500)]
        public string Location { get; set; }

        public float Points { get; set; }
        public int BookingPoints { get; set; }
        public float RegPoints { get; set; }
        public float WalletLimit { get; set; }


        [StringLength(50)]
        public string SMSID { get; set; }
        [StringLength(50)]
        public string SMSpassword { get; set; }
        [StringLength(50)]
        public string SMSusername { get; set; }
        [StringLength(500)]
        public string ShareText { get; set; }

        public bool IsActive { get; set; }
        
    }
}
