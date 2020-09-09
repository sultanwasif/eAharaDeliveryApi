using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class ShopMenu
    {
        public long Id { get; set; }
        [StringLength(200)]
        public string Image { get; set; }
        [StringLength(150)]
        public string Tittle { get; set; }
        public bool IsActive { get; set; }
        public long ShopId { get; set; }
        
        public virtual Shop Shop { get; set; }
    }
}
