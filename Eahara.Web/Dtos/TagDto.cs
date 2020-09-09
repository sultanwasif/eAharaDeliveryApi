using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class TagDto
    {

        public long Id { get; set; }
        public bool IsActive { get; set; }       
        public string Description { get; set; }
        public long? ShopId { get; set; }
        public virtual ShopDto Shop { get; set; }
        public long? ItemId { get; set; }
        public virtual ItemDto Item { get; set; }
    }
}