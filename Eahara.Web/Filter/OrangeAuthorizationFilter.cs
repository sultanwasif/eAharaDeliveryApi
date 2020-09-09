using Eahara.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Eahara.Web.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class OrangeAuthorizationFilter : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext filterContext)
        {
            var identity = FetchAuthHeader(filterContext);
            if (identity == null)
            {
                throw new UnauthorizedAccessException();
            }
            var genericPrincipal = new GenericPrincipal(identity, null);
            Thread.CurrentPrincipal = genericPrincipal;
            HttpContext.Current.User = genericPrincipal;
            base.OnAuthorization(filterContext);
        }
        protected virtual BasicAuthenticationIdentity FetchAuthHeader(HttpActionContext filterContext)
        {
            string Token = null;
            var authRequest = filterContext.Request.Headers.Authorization;
            if (authRequest != null && !String.IsNullOrEmpty(authRequest.Scheme) && authRequest.Scheme == "Basic")
                Token = authRequest.Parameter;
            if (string.IsNullOrEmpty(Token)) return null;
            var user = AuthenticationBL.IsTokenValid(Token);
            if (user != null)
            {
                var credentials = new BasicAuthenticationIdentity(user.UserName, Token, user.Id);
                return credentials;
            }
            return null;
        }
        private static void ChallengeAuthRequest(HttpActionContext filterContext)
        {
            var dnsHost = filterContext.Request.RequestUri.DnsSafeHost;
            filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            filterContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", dnsHost));
        }
    }
}