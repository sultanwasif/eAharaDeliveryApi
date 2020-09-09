using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class ReportDto
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public string RPTName { get; set; }
    }
}