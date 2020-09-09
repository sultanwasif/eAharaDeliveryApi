using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class MEDItem
    {
        public long Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(200)]
        public string Tagline { get; set; }
        public float Price { get; set; }
        public long? MEDOfferId { get; set; }
        public long MEDSubCategoryId { get; set; }
        public bool IsAvailable { get; set; }
        public float OfferPrice { get; set; }
        [StringLength(250)]
        public string Image1 { get; set; }
        [StringLength(250)]
        public string Image2 { get; set; }
        [StringLength(250)]
        public string Image3 { get; set; }
        [StringLength(250)]
        public string Image4 { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public long? MEDCategoryId { get; set; }
        public float Bookings { get; set; }

        public virtual MEDCategory MEDCategory { get; set; }
        public virtual MEDOffer MEDOffer { get; set; }
        public virtual MEDSubCategory MEDSubCategory { get; set; }

        public long? MEDBrandId { get; set; }
        public virtual MEDBrand MEDBrand { get; set; }

        public long? MEDShopId { get; set; }
        public virtual MEDShop MEDShop { get; set; }

    }
}
