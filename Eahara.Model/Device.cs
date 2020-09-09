using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class Device
    {
        public long Id { get; set; }
        [StringLength(250)]
        public string Platform { get; set; }
        [StringLength(250)]
        public string UUId { get; set; }
        [StringLength(150)]
        public string version { get; set; }
        [StringLength(50)]
        public string Language { get; set; }
    }
}
