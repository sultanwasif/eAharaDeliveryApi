using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class FAQ
    {
        public long Id { get; set; }
        [StringLength(200)]
        public string Question { get; set; }
        [StringLength(600)]
        public string Answer { get; set; }

        public bool IsActive { get; set; }
    }
}
