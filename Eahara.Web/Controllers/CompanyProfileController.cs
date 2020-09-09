using Eahara.Model;
using Eahara.Web.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Eahara.Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CompanyProfileController : ApiController
    {

        [HttpPost]
        [Route("SaveCompanyProfile")]
        public bool SaveCompanyProfile(CompanyProfileDto dataDto)
        {
            if (dataDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {

                    if (dataDto.Id > 0)
                    {
                        {
                            var data = context.CompanyProfiles.FirstOrDefault(x => x.Id == dataDto.Id);
                            if (data != null)
                            {
                                data.Address = dataDto.Address;
                                data.Name = dataDto.Name;
                                data.MobileNo = dataDto.MobileNo;
                                data.TeleNo = dataDto.TeleNo;
                                data.Email = dataDto.Email;
                                data.WhatsappNo = dataDto.WhatsappNo;
                                data.Location = dataDto.Location;
                                data.SMSID = dataDto.SMSID;
                                data.SMSpassword = dataDto.SMSpassword;
                                data.SMSusername = dataDto.SMSusername;
                                data.WalletLimit = dataDto.WalletLimit;
                                data.Points = dataDto.Points;
                                data.BookingPoints = dataDto.BookingPoints;
                                data.RegPoints = dataDto.RegPoints;
                                data.ShareText = dataDto.ShareText;


                                context.SaveChanges();
                                return true;
                            }
                            return false;
                        }
                    }
                    else
                    {
                        CompanyProfile company = new CompanyProfile();

                        company.Address = dataDto.Address;
                        company.Name = dataDto.Name;
                        company.MobileNo = dataDto.MobileNo;
                        company.TeleNo = dataDto.TeleNo;
                        company.Email = dataDto.Email;
                        company.WhatsappNo = dataDto.WhatsappNo;
                        company.Location = dataDto.Location;
                        company.Points = dataDto.Points;
                        company.BookingPoints = dataDto.BookingPoints;
                        company.RegPoints = dataDto.RegPoints;
                        company.WalletLimit = dataDto.WalletLimit;
                        company.ShareText = dataDto.ShareText;

                        company.SMSID = dataDto.SMSID;
                        company.SMSpassword = dataDto.SMSpassword;
                        company.SMSusername = dataDto.SMSusername;
                        context.CompanyProfiles.Add(company);

                        context.SaveChanges();
                        return true;
                    }

                }
            }
            return false;
        }

        [HttpGet]
        [Route("GetCompanyProfile")]
        public CompanyProfileDto GetCompanyProfile()
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataItem = context.CompanyProfiles.FirstOrDefault(x => x.Id > 0);
                if (dataItem != null)
                {
                    if (dataItem != null)
                    {
                        CompanyProfileDto returndata = new CompanyProfileDto();
                        returndata.Id = dataItem.Id;
                        returndata.Name = dataItem.Name;
                        returndata.WhatsappNo = dataItem.WhatsappNo;
                        returndata.TeleNo = dataItem.TeleNo;
                        returndata.Address = dataItem.Address;
                        returndata.Email = dataItem.Email;
                        returndata.MobileNo = dataItem.MobileNo;
                        returndata.Location = dataItem.Location;
                        returndata.RegPoints = dataItem.RegPoints;
                        returndata.BookingPoints = dataItem.BookingPoints;
                        returndata.Points = dataItem.Points;
                        returndata.WalletLimit = dataItem.WalletLimit;
                        returndata.ShareText = dataItem.ShareText;

                        returndata.SMSID = dataItem.SMSID;
                        returndata.SMSpassword = dataItem.SMSpassword;
                        returndata.SMSusername = dataItem.SMSusername;


                        return returndata;

                    }           

                }
                return null;
            }
        }


    }
}
