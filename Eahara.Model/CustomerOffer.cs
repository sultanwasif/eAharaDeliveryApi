using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class CustomerOffer
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        [StringLength(150)]
        public string RefNo { get; set; }
        public virtual Customer Customer { get; set; }
        public long PromoOfferId { get; set; }
        public virtual PromoOffer PromoOffer { get; set; }
    }
}
