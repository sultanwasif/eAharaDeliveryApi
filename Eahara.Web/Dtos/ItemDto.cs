using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class ItemDto
    {
        public long Id { get; set; }      
        public string Name { get; set; }       
        public string TagLine { get; set; }        
        public string Description { get; set; }       
        public float Price { get; set; }
        public float CommissionPercentage { get; set; }
        public float OfferPrice { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; }
        public bool InActive { get; set; }
        public long ShopId { get; set; }
        public virtual ShopDto Shop { get; set; }
        public long ItemCategoryId { get; set; }
        public virtual ItemCategoryDto ItemsCategory { get; set; }
        public virtual ICollection<ItemImageDto> ItemImages { get; set; }
        public long? OfferId { get; set; }
        public virtual OfferDto Offer { get; set; }
        public string Preference { get; set; }
        // for external purpose
        public int Quantity { get; set; }

    }
}