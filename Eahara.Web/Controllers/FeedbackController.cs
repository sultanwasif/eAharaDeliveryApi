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
    public class FeedbackController : ApiController
    {
        [HttpPost]
        [Route("AddFeedback")]
        public bool AddFeedback(FeedbackDto feedbackDto)
        {
            if (feedbackDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    Feedback feedback = new Feedback();

                    feedback.Description = feedbackDto.Description;
                    feedback.Name = feedbackDto.Name;
                    feedback.Designation = feedbackDto.Designation;
                    feedback.PhoneNo = feedbackDto.PhoneNo;
                    feedback.Satisfaction = feedbackDto.Satisfaction;
                    feedback.IsActive = true;
                    feedback.IsAccepted = false;
                    context.Feedbacks.Add(feedback);
                    context.SaveChanges();

                }
                return true;
            }
            return false;
        }
        [HttpPost]
        [Route("FeedbackInView")]

        public DataSourceResult FeedbackInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.Feedbacks.Where(x => x.IsActive == true)
                    .Select(x => new FeedbackDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Designation = x.Designation,
                        PhoneNo = x.PhoneNo,
                        Description = x.Description,
                        Satisfaction = x.Satisfaction,
                        IsAccepted = x.IsAccepted,
                    }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }

        [HttpGet]
        [Route("DeleteFeedback/{id}")]
        public bool deleteFeedback(int id)
        {
            using (EAharaDB context = new EAharaDB())
            {

                if (id != 0)
                {
                    var deleteFeedback = context.Feedbacks.FirstOrDefault(x => x.Id == id);
                    if (deleteFeedback != null)
                    {
                        deleteFeedback.IsActive = false;
                        context.Entry(deleteFeedback).Property(x => x.IsActive).IsModified = true;

                    }
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }

        [HttpGet]
        [Route("GetAllFeedback/{id}")]
        public List<FeedbackDto> GetAllFeedback(int id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Feedbacks.Where(x => x.IsActive == true)

                    .Select(x => new FeedbackDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        PhoneNo = x.PhoneNo,
                        Description = x.Description,
                        Designation = x.Designation,
                    }).OrderByDescending(x => x.Id).Skip(id).Take(3).ToList();

                return data;
            }
        }


    }
}
