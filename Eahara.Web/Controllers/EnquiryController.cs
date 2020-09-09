using Eahara.Model;
using Eahara.Web.Dtos;
using Kendo.DynamicLinq;
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
    public class EnquiryController : ApiController
    {
        [HttpPost]
        [Route("AddEnquiry")]
        public bool AddEnquiry(EnquiryDto dataDto)
        {
            if (dataDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    Enquiry addData = new Enquiry();

                    addData.Name = dataDto.Name;
                    addData.MobileNo = dataDto.MobileNo;
                    addData.Email = dataDto.Email;
                    addData.Remarks = dataDto.Remarks;
                    addData.Subject = dataDto.Subject;
                    addData.IsActive = true;
                    addData.IsClosed = false;
                    context.Enquiries.Add(addData);
                    context.SaveChanges();

                    return true;
                }
            }
            return false;
        }

        [HttpPost]
        [Route("EnquiriesInView")]
        public DataSourceResult EnquiriesInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.Enquiries.Where(x => x.IsActive == true)
                 .Select(x => new EnquiryDto
                 {
                     Id = x.Id,
                     IsActive = x.IsActive,
                     MobileNo = x.MobileNo,
                     Email = x.Email,
                     Subject = x.Subject,
                     IsClosed = x.IsClosed,
                     Remarks = x.Remarks,
                     Name = x.Name,
                 }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);
                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }

        

        [HttpGet]
        [Route("DeleteEnquiry/{id}")]
        public bool DeleteEnquiry(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var deleteData = context.Enquiries.FirstOrDefault(x => x.Id == id);
                    if (deleteData != null)
                    {
                        deleteData.IsActive = false;
                        context.Entry(deleteData).Property(x => x.IsActive).IsModified = true;
                    }
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }
    }
}
