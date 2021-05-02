using Eahara.Model;
using Eahara.Web.Dtos;
using Eahara.Web.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web.Http;

namespace Eahara.Web.Controllers
{
    public class MessageController : ApiController
    {


        public bool sendOTPSMS(string message, string mobno)
        {
            string msg = message;

            var temp = mobno;

            temp = mobno.ToString();
            if (temp.Length == 10)
            {
                temp = string.Concat("+91" + temp);
            }
            using (EAharaDB context = new EAharaDB())
            {
                var cp = context.CompanyProfiles.FirstOrDefault();

                string myParameters = "user=" + "eahara" + "&passwd= " + "bindas1234" + "&mobilenumber=" + temp + "&message=" + msg + "&sid=" + cp.SMSID + "&mtype=N&DR=Y";
                string URI = "http://api.smscountry.com/SMSCwebservice_bulk.aspx?";
                //string myParameters = "user=" + "sultan" + "&password= " + "5HT4R37G" + "&msisdn=" + temp + "&sid=" + "eAHARA" + "&msg=" + msg + "&fl=0&gwid=2";
                //string URI = "http://sms.planetweb.co.in/vendorsms/pushsms.aspx?";
                using (WebClient sms = new WebClient())
                {
                    sms.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    string HtmlResult = sms.UploadString(URI, myParameters);
                }
                return true;
            }
        }

        public bool sendSMS(string message, string mobno)
        {
            string msg = message;

            var temp = mobno;

            temp = mobno.ToString();
            if (temp.Length == 10)
            {
                temp = string.Concat("+91" + temp);
            }
            using (EAharaDB context = new EAharaDB())
            {
                var cp = context.CompanyProfiles.FirstOrDefault();

                string myParameters = "User=" + cp.SMSusername + "&passwd= " + cp.SMSpassword + "&mobilenumber=" + temp + "&message=" + msg + "&sid=" + cp.SMSID + "&mtype=N&DR=Y";
                string URI = "http://api.smscountry.com/SMSCwebservice_bulk.aspx?";
                using (WebClient sms = new WebClient())
                {
                    sms.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    string HtmlResult = sms.UploadString(URI, myParameters);
                }
                return true;
            }
        }

    }
}
