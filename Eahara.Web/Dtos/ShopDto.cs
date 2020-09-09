using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class ShopDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string TagLine { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string OpeningHours { get; set; }
        public string Cuisines { get; set; }
        public string Image { get; set; }
        public string AverageRating { get; set; }
        public long ShopCategoryId { get; set; }
        public string DeliveryTime { get; set; }
        public string Preference { get; set; }
        public float DeliveryCharge { get; set; }
        public float CommissionPercentage { get; set; }
        public string MobileNo { get; set; }
        public string MobileNo2 { get; set; }
        public string MobileNo3 { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public float DeliveryRange { get; set; }
        public float AverageCost { get; set; }
        public int Order { get; set; }

        public long? LocationId { get; set; }
        public virtual LocationDto Location { get; set; }

        public bool IsActive { get; set; }
        public virtual ShopCategoryDto ShopCategory { get; set; }

        public virtual ICollection<ShopImageDto> ShopImages { get; set; }
        public virtual ICollection<ShopInfoDto> ShopInfos { get; set; }
        public virtual ICollection<ShopMenuDto> ShopMenus { get; set; }
        public virtual ICollection<ItemDto> Items { get; set; }
    }
}