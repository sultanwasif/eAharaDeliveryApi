using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class MEDShopDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string TagLine { get; set; }
        public string MobileNo { get; set; }
        public string MobileNo2 { get; set; }
        public string MobileNo3 { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public float CommissionPercentage { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public bool IsActive { get; set; }
    }
}