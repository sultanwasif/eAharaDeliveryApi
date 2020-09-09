using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class PartnerDto
    {

        public long Id { get; set; }       
        public string Name { get; set; }       
        public string Image { get; set; }
        public bool IsActive { get; set; }
    }
}