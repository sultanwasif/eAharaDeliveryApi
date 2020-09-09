using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
   public class ShopInfo
    {
        public long Id { get; set; }
        [StringLength(400)]
        public string Description { get; set; }
        public long ShopId { get; set; }
        public bool IsActive { get; set; }
        public virtual Shop Shop { get; set; }
    }
}
