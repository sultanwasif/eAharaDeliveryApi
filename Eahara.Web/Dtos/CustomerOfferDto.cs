using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class CustomerOfferDto
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string RefNo { get; set; }
        public virtual CustomerDto Customer { get; set; }
        public long PromoOfferId { get; set; }
        public virtual PromoOfferDto PromoOffer { get; set; }
    }
}