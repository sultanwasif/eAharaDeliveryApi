using Eahara.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class UserSessionDto
    {
        public long Id { get; set; }
        public string Token { get; set; }
        public DateTime SessionTimeStamp { get; set; }
        public long ExpiresInMinutes { get; set; }
        public long UserId { get; set; }
        public virtual UserDto User { get; set; }
        public UserSessionStatus UserSessionStatus { get; set; }
    }
}