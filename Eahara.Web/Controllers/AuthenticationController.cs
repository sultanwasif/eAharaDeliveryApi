using Eahara.BL;
using Eahara.Model;
using Eahara.Web.Dtos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Eahara.Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AuthenticationController : ApiController
    {
        [HttpPost]
        [Route("Login")]
        public TokenInfoDto LoginWithUserName(UserDto loginInfo)
        {
            TokenInfoDto sessionDto = new TokenInfoDto();
            if (loginInfo.UserName != null && loginInfo.Password != null)
            {
                var userSession = AuthenticationBL.LoginWithUserName(loginInfo.UserName, loginInfo.Password);
                if (userSession != null)
                {
                    sessionDto.Role = userSession.User.Role;
                    sessionDto.UserId = userSession.User.Id;
                    sessionDto.UserName = userSession.User.UserName;     
                    sessionDto.Token = userSession.Token;
                    sessionDto.ShopId = userSession.User.ShopId;
                    sessionDto.EmployeeId = userSession.User.EmployeeId;
                    sessionDto.CustomerId = userSession.User.CustomerId;
                    sessionDto.MEDShopId = userSession.User.MEDShopId;
                    return sessionDto;
                }
            }
            return null;
        }

        [HttpPost]
        [Route("LogOut")]
        public bool LogOutWithToken(TokenInfoDto tokenInfo)
        {
            if (tokenInfo != null)
            {
                return AuthenticationBL.LogOutWithToken(tokenInfo.Token);
            }
            return false;
        }

        [HttpPost]
        [Route("ChangePassword")]
        public bool ChangePassword(UserDto userDto)
        {
            if (userDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    //var token = Request.Headers.Authorization.Parameter;
                    //User User = AuthenticationBL.IsTokenValid(token);
                    var user = context.Users.FirstOrDefault(X => X.Id == userDto.Id);
                    var passwordSalt = AuthenticationBL.CreatePasswordSalt(Encoding.ASCII.GetBytes(userDto.Password));
                    user.PasswordSalt = Convert.ToBase64String(passwordSalt);
                    var password = AuthenticationBL.CreateSaltedPassword(passwordSalt, Encoding.ASCII.GetBytes(userDto.Password));
                    user.Password = Convert.ToBase64String(password);
                    context.Entry(user).State = EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }
    }
}
