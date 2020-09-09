using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
  public  class Notification
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public virtual User User { get; set; }
        [StringLength(400)]
        public string Description { get; set; }
        public bool IsViewed { get; set; }
        public bool IsActive { get; set; }


    }
}
