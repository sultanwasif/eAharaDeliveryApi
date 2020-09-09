using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
  public  class Tag
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }
        [StringLength(400)]
        public string Description { get; set; }    
        public long? ShopId { get; set; }
        public virtual Shop Shop { get; set; }
        public long? ItemId { get; set; }
        public virtual Item Item { get; set; }
    }
}
