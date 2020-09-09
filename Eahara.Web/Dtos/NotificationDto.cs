using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class NotificationDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public virtual UserDto User { get; set; }      
        public string Description { get; set; }
        public bool IsViewed { get; set; }
        public bool IsActive { get; set; }

    }
}