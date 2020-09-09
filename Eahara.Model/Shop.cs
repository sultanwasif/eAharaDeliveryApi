using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
   public class Shop
    {
        public long Id { get; set; }  
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(150)]
        public string TagLine { get; set; }
        [StringLength(150)]
        public string MobileNo { get; set; }
        [StringLength(150)]
        public string MobileNo2 { get; set; }
        [StringLength(150)]
        public string MobileNo3 { get; set; }
        [StringLength(250)]
        public string Address { get; set; }
        [StringLength(400)]
        public string Description { get; set; }
        public float AverageCost { get; set; }
        [StringLength(100)]
        public string OpeningHours { get; set; }
        [StringLength(200)]
        public string Cuisines { get; set; }
        [StringLength(200)]
        public string Image { get; set; }
        [StringLength(150)]
        public string AverageRating { get; set; }
        [StringLength(50)]
        public string DeliveryTime { get; set; }
        [StringLength(100)]
        public string Preference { get; set; }
        public float DeliveryCharge { get; set; }
        public float CommissionPercentage { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        [StringLength(150)]
        public string Lat { get; set; }
        [StringLength(150)]
        public string Lng { get; set; }
        public float DeliveryRange { get; set; }
        public int Order { get; set; }        
        public long ShopCategoryId { get; set; }

        public bool IsActive { get; set; }

        public long? LocationId { get; set; }
        public virtual Location Location { get; set; }

        public virtual ShopCategory ShopCategory { get; set; }

        public virtual ICollection<ShopImage> ShopImages { get; set; }
        public virtual ICollection<ShopInfo> ShopInfos { get; set; }
        public virtual ICollection<ShopMenu> ShopMenus { get; set; }
        public virtual ICollection<Item> Items { get; set; }
    }
}
