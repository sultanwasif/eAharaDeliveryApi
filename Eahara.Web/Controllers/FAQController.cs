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
    public class FAQController : ApiController
    {
        [HttpPost]
        [Route("AddFAQ")]
        public bool AddFAQ(FAQDto faqdto)
        {
            if (faqdto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (faqdto.Id > 0)
                    {
                        var data = context.FAQs.FirstOrDefault(x => x.Id == faqdto.Id);
                        if (data != null)
                        {
                            data.Question = faqdto.Question;
                            data.Answer = faqdto.Answer;
                            context.Entry(data).Property(x => x.Question).IsModified = true;
                            context.Entry(data).Property(x => x.Answer).IsModified = true;
                            

                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        FAQ faq = new FAQ();

                        faq.Question = faqdto.Question;
                        faq.Answer = faqdto.Answer;
                        faq.IsActive = true;
                        context.FAQs.Add(faq);

                        context.SaveChanges();
                        return true;
                    }

                }

            }
            return false;
        }
        
        [HttpPost]
        [Route("FAQInView")]

        public DataSourceResult FAQInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.FAQs.Where(x => x.IsActive == true)
                    .Select(x => new FAQDto
                    {
                        Id = x.Id,
                        Answer = x.Answer,
                        IsActive = x.IsActive,
                        Question = x.Question,
                        
                    }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }

        [HttpGet]
        [Route("DeleteFAQ/{id}")]
        public bool DeleteFAQ(int id)
        {
            using (EAharaDB context = new EAharaDB())
            {

                if (id != 0)
                {
                    var faq = context.FAQs.FirstOrDefault(x => x.Id == id);
                    if (faq != null)
                    {
                       faq.IsActive = false;
                        context.Entry(faq).Property(x => x.IsActive).IsModified = true;

                    }
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }

        [HttpGet]
        [Route("FAQInHome")]
        public List<FAQDto> FAQInHome()
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.FAQs.Where(x => x.IsActive)
                    .Select(x => new FAQDto
                    {
                        Id = x.Id,
                        Question = x.Question,
                        Answer = x.Answer,
                    }).OrderByDescending(x => x.Id).ToList();

                return data;
            }
        }

    }
}


