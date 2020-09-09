using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class ShopInfoDto
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public long ShopId { get; set; }
        public bool IsActive { get; set; }
        public virtual ShopDto Shop { get; set; }
    }
}