using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class PromoOffer
    {
        public long Id { get; set; }
        public float Value { get; set; }
        public float MaxValue { get; set; }
        public bool IsPercentage { get; set; }
        [StringLength(150)]
        public string Code { get; set; }
        [StringLength(250)]
        public string Image { get; set; }
        [StringLength(200)]
        public string Tittle { get; set; }
        public int Count { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<CustomerOffer> CustomerOffers { get; set; }

    }
}
