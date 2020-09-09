using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class EnquiryDto
    {

        public long Id { get; set; }
     
        public string Name { get; set; }
       
        public string MobileNo { get; set; }
       
        public string Email { get; set; }
   
        public string Subject { get; set; }
        
        public string Remarks { get; set; }

        public bool IsActive { get; set; }

        public bool IsClosed { get; set; }
    }
}