using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class CompanyProfileDto
    {
        public long Id { get; set; }    
        public string Name { get; set; }       
        public string Address { get; set; }        
        public string Email { get; set; }      
        public string MobileNo { get; set; }     
        public string TeleNo { get; set; }
        public string WhatsappNo { get; set; }
        public string Location { get; set; }

        public float Points { get; set; }
        public int BookingPoints { get; set; }
        public float RegPoints { get; set; }
        public float WalletLimit { get; set; }


        public string SMSID { get; set; } 
        public string SMSpassword { get; set; } 
        public string SMSusername { get; set; }
        public string ShareText { get; set; }

        public bool IsActive { get; set; }
    }
}