using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class FeedbackDto
    {

        public long Id { get; set; }
        
        public string Name { get; set; }
       
        public string Designation { get; set; }
        
        public string PhoneNo { get; set; }
       
        public string Description { get; set; }

        public int Satisfaction { get; set; }

        public bool IsActive { get; set; }

        public bool IsAccepted { get; set; }
    }
}