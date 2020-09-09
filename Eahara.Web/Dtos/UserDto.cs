using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class UserDto
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string PasswordSalt { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsNotSkip { get; set; }
        public string Role { get; set; }
        public bool IsBlocked { get; set; }
        public string Type { get; set; }

        public long? EmployeeId { get; set; }
        public long? ShopId { get; set; }
        public long? CustomerId { get; set; }
        public long? MEDShopId { get; set; }

        public virtual EmployeeDto Employee { get; set; }
        public virtual ShopDto Shop { get; set; }
        public virtual MEDShopDto MEDShop { get; set; }

    }
}