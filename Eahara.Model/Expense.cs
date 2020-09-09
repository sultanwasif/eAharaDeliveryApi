using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class Expense
    {
        public long Id { get; set; }
        [StringLength(120)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
