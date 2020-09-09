using Eahara.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.BL
{
    public class AuthenticationBL
    {
        public static User IsTokenValid(string token)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var session = context.UserSessions.FirstOrDefault(x => x.Token == token);

                if (session != null)
                {
                    if (session.SessionTimeStamp + TimeSpan.FromMinutes(session.ExpiresInMinutes) >= DateTime.Now)
                    {
                        return session.User;
                    }
                }
                return null;
            }
        }

        public static UserSession LoginWithUserName(string userName, string password)
        {
            UserSession userSession = null;
            using (EAharaDB context = new EAharaDB())
            {
                var user = context.Users.FirstOrDefault(x => x.UserName == userName && x.IsActive);

                if (user != null)
                {
                    var storedPassword = Convert.FromBase64String(user.Password);
                    var a = ComparePasswords(storedPassword, Encoding.ASCII.GetBytes(password));
                    if (ComparePasswords(storedPassword, Encoding.ASCII.GetBytes(password)))
                    {
                        string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                        userSession = new UserSession();
                        userSession.ExpiresInMinutes = 480;
                        userSession.SessionTimeStamp = DateTime.Now;
                        userSession.Token = token;
                        userSession.UserId = user.Id;
                        context.UserSessions.Add(userSession);
                        context.SaveChanges();
                        userSession.User = context.Users
                        .FirstOrDefault(x => x.Id == userSession.UserId);
                        return userSession;
                    }
                    else
                    {
                        throw new WebFaultException(HttpStatusCode.Forbidden);
                    }
                }
                return null;

            }


        }
        public static bool LogOutWithToken(string token)
        {
            using (EAharaDB context = new EAharaDB())
            {
                UserSession userSession = context.UserSessions.FirstOrDefault(x => x.Token == token);
                if (null != userSession)
                {
                    userSession.UserSessionStatus = UserSessionStatus.LoggedOut;
                    userSession.SessionTimeStamp = DateTime.Now;
                    context.Entry(userSession).State = EntityState.Modified;
                    context.SaveChanges();
                }
                return true;
            }
        }

        public static byte[] CreatePasswordSalt(byte[] unsaltedPassword)
        {
            byte[] saltValue = new byte[64];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(saltValue);

            return saltValue;
        }

        public static byte[] CreateSaltedPassword(byte[] saltValue, byte[] unsaltedPassword)
        {

            byte[] rawSalted = new byte[unsaltedPassword.Length + saltValue.Length];
            unsaltedPassword.CopyTo(rawSalted, 0);
            saltValue.CopyTo(rawSalted, unsaltedPassword.Length);


            SHA1 sha1 = SHA1.Create();
            byte[] saltedPassword = sha1.ComputeHash(rawSalted);


            byte[] dbPassword = new byte[saltedPassword.Length + saltValue.Length];
            saltedPassword.CopyTo(dbPassword, 0);
            saltValue.CopyTo(dbPassword, saltedPassword.Length);

            return dbPassword;
        }


        private static bool ComparePasswords(byte[] storedPassword, byte[] hashedPassword)
        {
            if (storedPassword == null || hashedPassword == null)
                return false;
            byte[] saltValue = new byte[64];
            int saltOffset = storedPassword.Length - 64;
            for (int i = 0; i < 64; i++)
                saltValue[i] = storedPassword[saltOffset + i];

            byte[] saltedPassword = CreateSaltedPassword(saltValue, hashedPassword);
            return CompareByteArray(storedPassword, saltedPassword);
        }

        private static bool CompareByteArray(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
                return false;
            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                    return false;
            }
            return true;
        }


        [Serializable]
        internal class WebFaultException : Exception
        {
            private HttpStatusCode forbidden;

            public WebFaultException()
            {
            }

            public WebFaultException(string message) : base(message)
            {
            }

            public WebFaultException(HttpStatusCode forbidden)
            {
                this.forbidden = forbidden;
            }

            public WebFaultException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected WebFaultException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }

        public int GenerateUserOtp(string MobileNo)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var userOtp = context.UserOTPs.FirstOrDefault(x => x.MobileNo == MobileNo);
                if (userOtp != null)
                {
                    Random rnd = new Random();
                    int otp = rnd.Next(1000, 9999);
                    userOtp.OTP = otp;
                    userOtp.Date = DateTime.Now;
                    context.Entry(userOtp).State = EntityState.Modified;
                    context.SaveChanges();
                    return otp;
                }
                else
                {
                    userOtp = new UserOTP();
                    Random rnd = new Random();
                    int otp = rnd.Next(1000, 9999);
                    userOtp.MobileNo = MobileNo;
                    userOtp.OTP = otp;
                    userOtp.Date = DateTime.Now;
                    context.UserOTPs.Add(userOtp);
                    context.SaveChanges();
                    return otp;
                }

            }
        }
    }
}
