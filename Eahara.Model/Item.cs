using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class Item
    {

        public long Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(150)]
        public string TagLine { get; set; }        
        [StringLength(400)]
        public string Description { get; set; }
        public float Price { get; set; }     
        public float OfferPrice { get; set; }     
        [StringLength(200)]
        public string Image { get; set; }
        public bool IsActive { get; set; }
        public float CommissionPercentage { get; set; }
        public bool InActive{ get; set; }
        [StringLength(100)]
        public string Preference { get; set; }

        public long ShopId { get; set; }
        public virtual Shop Shop { get; set; }
        public long ItemCategoryId { get; set; }
        public virtual ItemCategory ItemsCategory { get; set; }

        public long? OfferId { get; set; }
        public virtual Offer Offer { get; set; }

        public virtual ICollection<ItemImage> ItemImages { get; set; }

    }
}
