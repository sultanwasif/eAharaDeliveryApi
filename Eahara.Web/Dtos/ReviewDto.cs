using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class ReviewDto
    {
        public long Id { get; set; }
        public int Rating { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
        public long ShopId { get; set; }
        public bool IsActive { get; set; }
        public virtual ShopDto Shop { get; set; }
    }
}