using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Eahara.Web.Filter
{
    public class BasicAuthenticationIdentity : GenericIdentity
    {
        public string UserName
        {
            get;
            set;
        }
        public string Token
        {
            get;
            set;
        }
        public long Id
        {
            get;
            set;
        }
        public BasicAuthenticationIdentity(string userName, string token, long id) : base(userName, "Basic")
        {
            UserName = userName;
            Token = token;
            Id = id;
        }
    }
}