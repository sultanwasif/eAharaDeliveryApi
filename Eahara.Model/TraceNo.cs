using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
   public class TraceNo
    {
        public long Id { get; set; }
        [StringLength(60)]
        public string Type { get; set; }
        [StringLength(100)]
        public string Prefix { get; set; }
        public long Number { get; set; }



    }
}
