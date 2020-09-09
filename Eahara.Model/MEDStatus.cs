using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class MEDStatus
    {
        public long Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(250)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
