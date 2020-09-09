using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class MEDShop
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
        [StringLength(200)]
        public string Image { get; set; }
        public float CommissionPercentage { get; set; }
        [StringLength(150)]
        public string Lat { get; set; }
        [StringLength(150)]
        public string Lng { get; set; }
        public bool IsActive { get; set; }
    }
}
