using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class Report
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        [StringLength(200)]
        public string RPTName { get; set; }
    }
}
