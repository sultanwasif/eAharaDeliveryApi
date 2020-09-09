using Eahara.BL;
using Eahara.Model;
using Eahara.Web.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Eahara.Web.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class RegisterController : ApiController
    {
        [HttpGet]
        [Route("SendRegisterOtp/{MobileNo}")]
        public int SendRegisterOtp(string MobileNo)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (MobileNo != "" && MobileNo != null)
                {
                    var old = context.Customers.FirstOrDefault(x => x.IsActive && x.MobileNo == MobileNo);
                    if (old != null)
                    {
                        return 2;
                    }

                    AuthenticationBL authBl = new AuthenticationBL();
                    int Otp = authBl.GenerateUserOtp(MobileNo);
                    if (Otp > 0)
                    {
                        MessageController messagectrl = new MessageController();

                        string msg = Otp + " is your Eahara verification code no, which expires in few minutes.";

                        messagectrl.sendOTPSMS(msg, MobileNo);
                        return Otp;
                    }
                }
            }
            return 0;
        }
               
        [HttpGet]
        [Route("SendTroubleOtp/{MobileNo}")]
        public SmsPostDto SendTroubleOtp(string MobileNo)
        {
            using (EAharaDB context = new EAharaDB())
            {
                SmsPostDto smsdto = new SmsPostDto();
                if (MobileNo != "" && MobileNo != null)
                {
                    var old = context.Customers.FirstOrDefault(x => x.IsActive && x.MobileNo == MobileNo);
                    if (old == null)
                    {
                        smsdto.Message = "NotRegistered";
                        return smsdto;
                    }

                    AuthenticationBL authBl = new AuthenticationBL();
                    int Otp = authBl.GenerateUserOtp(MobileNo);
                    if (Otp > 0)
                    {
                        MessageController messagectrl = new MessageController();

                        string msg = Otp + " is your Eahara verification code no, which expires in few minutes.";

                        messagectrl.sendOTPSMS(msg, MobileNo);

                        var user = context.Users.FirstOrDefault(x => x.CustomerId == old.Id);
                        if (user != null)
                        {
                            smsdto.OTP = Otp;
                            smsdto.UserId = user.Id;
                            smsdto.Message = "Done";
                            return smsdto;
                        }
                        else
                        {
                            smsdto.Message = "NoUser";
                            return smsdto;
                        }
                    }
                }
            }
            return null;
        }

        [HttpPost]
        [Route("RegisterCustomer")]
        public int RegisterCustomer(CustomerDto dataDto)
        {
            if (dataDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var oldusr = context.Users.FirstOrDefault(x => x.IsActive && x.UserName == dataDto.UserName);
                            if (oldusr != null)
                            {
                                return 2;
                            }

                            Customer cus = new Customer();

                            cus.Name = dataDto.Name;
                            cus.Email = dataDto.Email;
                            cus.MobileNo = dataDto.MobileNo;
                            cus.TelephoneNo = dataDto.TelephoneNo;
                            cus.Location = dataDto.Location;
                            cus.CreatedDate = DateTime.Now;
                            cus.Address = dataDto.Address;
                            cus.Photo = dataDto.Photo;
                            cus.RefNo = dataDto.RefNo;
                            cus.InstRefNo = dataDto.InstRefNo;
                            cus.Points = dataDto.Points;
                            cus.IsActive = true;

                            var traceNumber = context.TraceNoes.FirstOrDefault(x => x.Type == "CU");
                            if (traceNumber == null)
                            {
                                traceNumber = new TraceNo();
                                traceNumber.Type = "CU";
                                traceNumber.Number = 10001;
                                context.TraceNoes.Add(traceNumber);
                            }
                            else
                            {
                                traceNumber.Number += 1;
                                context.Entry(traceNumber).Property(x => x.Number).IsModified = true;
                            }
                            cus.RefNo = traceNumber.Type + traceNumber.Number;

                            if (dataDto.InstRefNo != null && dataDto.InstRefNo != "")
                            {
                                var oldClient = context.Customers.FirstOrDefault(x => x.IsActive && x.RefNo == dataDto.InstRefNo);

                                if (oldClient != null)
                                {
                                    oldClient.Points = oldClient.Points + context.CompanyProfiles.FirstOrDefault().Points;
                                    context.Entry(oldClient).Property(x => x.Points).IsModified = true;
                                }

                            }

                            cus.Points = cus.Points + context.CompanyProfiles.FirstOrDefault().RegPoints;

                            context.Customers.Add(cus);
                            context.SaveChanges();

                            if (dataDto.CustomerMMethods.Count() > 0)
                            {
                                foreach(var mm in dataDto.CustomerMMethods)
                                {
                                    CustomerMMethod cmm = new CustomerMMethod();
                                    cmm.CustomerId = cus.Id;
                                    cmm.MMethodId = mm.MMethodId;
                                    cmm.IsActive = true;

                                    context.CustomerMMethods.Add(cmm);
                                }
                            }

                            Address add = new Address();
                            add.CustomerId = cus.Id;
                            add.Description = cus.Address;
                            add.Location = cus.Location;
                            add.Title = "Default";
                            context.Addresses.Add(add);
                            context.SaveChanges();

                            User usr = new User();

                            usr.UserName = dataDto.UserName;
                            var passwordSalt = AuthenticationBL.CreatePasswordSalt(Encoding.ASCII.GetBytes(dataDto.Password));
                            usr.PasswordSalt = Convert.ToBase64String(passwordSalt);
                            var password = AuthenticationBL.CreateSaltedPassword(passwordSalt, Encoding.ASCII.GetBytes(dataDto.Password));
                            usr.Password = Convert.ToBase64String(password);
                            usr.CustomerId = cus.Id;
                            usr.Role = "Customer";
                            usr.IsActive = true;

                            context.Users.Add(usr);
                            context.SaveChanges();
                            transaction.Commit();

                            return 1;

                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            return 0;
                        }
                    }

                }

            }
            return 0;
        }

        [HttpPost]
        [Route("ResetTroublePswd")]
        public bool ResetTroublePswd(UserDto dataDto)
        {
            if (dataDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    var oldusr = context.Users.FirstOrDefault(x => x.IsActive && x.Id == dataDto.Id);
                    if (oldusr != null)
                    {
                        var passwordSalt = AuthenticationBL.CreatePasswordSalt(Encoding.ASCII.GetBytes(dataDto.Password));
                        oldusr.PasswordSalt = Convert.ToBase64String(passwordSalt);
                        var password = AuthenticationBL.CreateSaltedPassword(passwordSalt, Encoding.ASCII.GetBytes(dataDto.Password));
                        oldusr.Password = Convert.ToBase64String(password);
                        
                        context.SaveChanges();
                        return true;
                    }
                   

                }

            }
            return false;
        }

    }
}
