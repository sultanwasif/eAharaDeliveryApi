using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class ShopImageDto
    {
        public long Id { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public long ShopId { get; set; }
        public bool IsActive { get; set; }
        public virtual ShopDto Shop { get; set; }
    }
}