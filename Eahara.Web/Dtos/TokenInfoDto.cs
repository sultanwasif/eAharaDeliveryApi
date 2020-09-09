using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class TokenInfoDto
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public long UserId { get; set; }
        public long? ShopId { get; set; }
        public long? EmployeeId { get; set; }
        public long? CustomerId { get; set; }
        public long? MEDShopId { get; set; }
    }
}