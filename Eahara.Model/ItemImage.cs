using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
   public class ItemImage
    {
        public long Id { get; set; }
        [StringLength(250)]
        public string Image { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        public long ItemId { get; set; }
        public bool IsActive { get; set; }
        public virtual Item Item { get; set; }
    }
}
