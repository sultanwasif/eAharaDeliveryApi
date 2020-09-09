using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class MEDOffer
    {
        public long Id { get; set; }
        public float Percentage { get; set; }
        public bool IsPercentage { get; set; }
        [StringLength(250)]
        public string Image { get; set; }
        [StringLength(200)]
        public string Title { get; set; }
        public bool IsActive { get; set; }

        public long? MEDShopId { get; set; }
        public virtual MEDShop MEDShop { get; set; }
    }
}
