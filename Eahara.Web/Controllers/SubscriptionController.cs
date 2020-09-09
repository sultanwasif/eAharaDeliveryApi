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
    public class SubscriptionController : ApiController
    {
        [HttpPost]
        [Route("AdSubscription")]
        public bool AddSubscription(SubscriptionDto subscriptionDto)
        {
            if (subscriptionDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    Subscription subscription = new Subscription();

                    subscription.EmailId = subscriptionDto.EmailId;
                    subscription.Name = subscriptionDto.Name;
                    
                   
                    subscription.IsActive = true;
                   
                    context.Subscriptions.Add(subscription);
                    context.SaveChanges();

                }
                return true;
            }
            return false;
        }
        [HttpPost]
        [Route("SubscriptionInView")]

        public DataSourceResult SubscriptionInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.Subscriptions.Where(x => x.IsActive == true)
                    .Select(x => new SubscriptionDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        EmailId = x.EmailId,
                        
                    }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }

        [HttpGet]
        [Route("DeleteSubscription/{id}")]
        public bool DeleteSubscription(int id)
        {
            using (EAharaDB context = new EAharaDB())
            {

                if (id != 0)
                {
                    var deletesubscription = context.Subscriptions.FirstOrDefault(x => x.Id == id);
                    if (deletesubscription != null)
                    {
                        deletesubscription.IsActive = false;
                        context.Entry(deletesubscription).Property(x => x.IsActive).IsModified = true;

                    }
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }

    }
}
