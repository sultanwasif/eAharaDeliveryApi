using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
   public class Review
    {
        public long Id { get; set; }
        public int Rating { get; set; }
        [StringLength(400)]
        public string Description { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(100)]
        public string MobileNo { get; set; }
        [StringLength(100)]
        public string EmailId { get; set; }
        public long ShopId { get; set; }
        public bool IsActive { get; set; }
        public virtual Shop Shop { get; set; }
    }
}
