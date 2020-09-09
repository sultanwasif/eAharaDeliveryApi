using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class MEDItemDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Tagline { get; set; }
        public float Price { get; set; }
        public long? MEDOfferId { get; set; }
        public long MEDSubCategoryId { get; set; }
        public bool IsAvailable { get; set; }
        public float OfferPrice { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public long? MEDCategoryId { get; set; }

        public virtual MEDCategoryDto MEDCategory { get; set; }
        public float Bookings { get; set; }
        public virtual MEDOfferDto MEDOffer { get; set; }
        public virtual MEDSubCategoryDto MEDSubCategory { get; set; }

        public long? MEDBrandId { get; set; }
        public virtual MEDBrandDto MEDBrand { get; set; }

        public long? MEDShopId { get; set; }
        public virtual MEDShopDto MEDShop { get; set; }
    }
}