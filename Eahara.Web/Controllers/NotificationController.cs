using Eahara.Model;
using Eahara.Web.Dtos;
using Kendo.DynamicLinq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Eahara.Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class NotificationController : ApiController
    {
        public bool addShopNotification(string msg, long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var user = context.Users.FirstOrDefault(x=>x.ShopId == id);
                if (user != null)
                {
                    Notification noti = new Notification();
                    noti.UserId = user.Id;
                    noti.Description = msg;
                    noti.IsActive = true;
                    context.Notifications.Add(noti);
                    context.SaveChanges();
                }
            }
            return true;
        }

        public bool addCustomerNotification(string msg, long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var user = context.Users.FirstOrDefault(x => x.CustomerId == id);
                if (user != null)
                {
                    Notification noti = new Notification();
                    noti.UserId = user.Id;
                    noti.Description = msg;
                    noti.IsActive = true;
                    context.Notifications.Add(noti);
                    context.SaveChanges();
                }
            }
            return true;
        }

        public bool addAdminNotification(string msg)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var user = context.Users.FirstOrDefault(x => x.Role == "Admin");
                if (user != null)
                {
                    Notification noti = new Notification();
                    noti.UserId = user.Id;
                    noti.Description = msg;
                    noti.IsActive = true;
                    context.Notifications.Add(noti);
                    context.SaveChanges();
                }
            }
            return true;
        }

        public bool addEmployeeNotification(string msg , long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var user = context.Users.FirstOrDefault(x => x.Id == id);
                if (user != null)
                {
                    Notification noti = new Notification();
                    noti.UserId = user.Id;
                    noti.Description = msg;
                    noti.IsActive = true;
                    context.Notifications.Add(noti);
                    context.SaveChanges();
                }
            }
            return true;
        }

        [HttpGet]
        [Route("GetUserNotifications/{id}")]
        public List<NotificationDto> GetUserNotifications(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var dataList = context.Notifications.Where(x => x.UserId == id && x.IsActive)
                        .Select(x => new NotificationDto
                        {
                            Id = x.Id,
                            UserId = x.UserId,
                            Description = x.Description,
                            IsViewed = x.IsViewed,
                        }).OrderByDescending(x => x.Id).Take(20).ToList();
              
                    return dataList;
                }
                return null;
            }
        }


        [HttpGet]
        [Route("resetUserNotifications/{id}")]
        public bool resetUserNotifications(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var dataList = context.Notifications.Where(x => x.UserId == id && x.IsActive && !x.IsViewed).ToList();

                    foreach (var n in dataList)
                    {
                        n.IsViewed = true;
                        context.Entry(n).Property(x => x.IsViewed).IsModified = true;
                        context.SaveChanges();
                    }
                }
                return true;
            }
        }

        [HttpGet]
        [Route("getNotiCaount/{id}")]
        public int getNotiCaount(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var dataList = context.Notifications.Where(x => x.UserId == id && x.IsActive && !x.IsViewed).Count();

                    return dataList;
                }
                return 0;
            }
        }

    }
}
