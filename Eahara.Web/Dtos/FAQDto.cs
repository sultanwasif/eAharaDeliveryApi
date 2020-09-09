using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class FAQDto
    {
        public long Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool IsActive { get; set; }
    }
}