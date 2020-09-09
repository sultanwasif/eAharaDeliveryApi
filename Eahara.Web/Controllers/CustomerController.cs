using Eahara.Model;
using Eahara.Web.Dtos;
using Kendo.DynamicLinq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Eahara.Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CustomerController : ApiController
    {

        [HttpPost]
        [Route("CustomersInView")]
        public DataSourceResult CustomersInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.Customers.Where(x => x.IsActive == true)
                    .Select(x => new CustomerDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive, 
                        Email = x.Email,
                        MobileNo = x.MobileNo,
                        TelephoneNo = x.TelephoneNo,
                        Address = x.Address,
                        Location = x.Location,
                        BLCount = x.BLCount,
                        CreatedDate = x.CreatedDate, 
                        RefNo = x.RefNo,
                        Points = x.Points,
                        InstRefNo = x.InstRefNo,
                        Photo = x.Photo,
                    }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }


        [HttpPost]
        [Route("CustomerReportsInView")]
        public DataSourceResult CustomerReportsInView(KendoFilterRequestDto Request)
        {
            using (EAharaDB context = new EAharaDB())
            {

                var rep = context.Customers.Where(x => x.IsActive &&
                                               ((DbFunctions.TruncateTime(x.CreatedDate)) >= (DbFunctions.TruncateTime(Request.FromDate))
                                              && (DbFunctions.TruncateTime(x.CreatedDate)) <= (DbFunctions.TruncateTime(Request.ToDate))));

        var dataSourceResult = rep.Select(x => new CustomerDto
        {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive, 
                        Email = x.Email,
                        MobileNo = x.MobileNo,
                        TelephoneNo = x.TelephoneNo,
                        Address = x.Address,
                        Location = x.Location, 
                        CreatedDate = x.CreatedDate, 
                        RefNo = x.RefNo,
                        Points = x.Points,
                        BLCount = x.BLCount,
                        InstRefNo = x.InstRefNo,
                        Photo = x.Photo,
                        TOrders = context.Bookings.Where(y=>y.IsActive && y.CustomerId == x.Id).Count(),
                    }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }

        [HttpGet]
        [Route("CustomerById/{id}")]
        public CustomerDto CustomerById(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.Customers.Where(x => x.Id == id)
                    .Select(x => new CustomerDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Email = x.Email,
                        MobileNo = x.MobileNo,
                        TelephoneNo = x.TelephoneNo,
                        Address = x.Address,
                        Location = x.Location,
                        CreatedDate = x.CreatedDate,
                        RefNo = x.RefNo,
                        Points = x.Points,
                        BLCount = x.BLCount,
                        InstRefNo = x.InstRefNo,
                        Photo = x.Photo,
                    }).OrderByDescending(x => x.Id).FirstOrDefault();

                return dataSourceResult;
            }
        }

        [HttpGet]
        [Route("DeleteCustomerById/{id}")]
        public bool DeleteCustomerById(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.Customers.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {
                        Delete.IsActive = false;
                        context.Entry(Delete).Property(x => x.IsActive).IsModified = true;

                    }
                    context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        [HttpGet]
        [Route("AddCustomerWallet/{id}/{wallet}")]
        public bool AddCustomerWallet(long id, float wallet)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.Customers.FirstOrDefault(x => x.Id == id);
                    if (Delete != null && wallet!=0)
                    {
                        Delete.Points = Delete.Points + wallet;
                        context.Entry(Delete).Property(x => x.Points).IsModified = true;

                        NotificationController Notictrl = new NotificationController();
                        Notictrl.addCustomerNotification(wallet + " RS Added To Your Wallet", Delete.Id);

                    }
                    context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        [HttpPost]
        [Route("updateCustomer")]
        public bool updateCustomer(CustomerDto dataDto)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Customers.FirstOrDefault(x => x.Id == dataDto.Id);

                data.Name = dataDto.Name;
                data.Email = dataDto.Email;
                data.TelephoneNo = dataDto.TelephoneNo;

                context.Entry(data).Property(x => x.Name).IsModified = true;
                context.Entry(data).Property(x => x.Email).IsModified = true;
                context.Entry(data).Property(x => x.TelephoneNo).IsModified = true;
                context.SaveChanges();
                return true;
            }
        }


        [HttpPost]
        [Route("AddCustomer")]
        public bool AddCustomer(CustomerDto dataDto)
        {
            if (dataDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (dataDto.Id > 0)
                    {
                        var data = context.Customers.FirstOrDefault(x => x.Id == dataDto.Id);
                        if (data != null)
                        {
                            data.Name = dataDto.Name;
                            data.IsActive = dataDto.IsActive;
                            data.Email = dataDto.Email;
                            data.MobileNo = dataDto.MobileNo;
                            data.TelephoneNo = dataDto.TelephoneNo;
                            data.Address = dataDto.Address;
                            data.Location = dataDto.Location;
                            data.CreatedDate = dataDto.CreatedDate;
                            data.RefNo = dataDto.RefNo;
                            data.Points = dataDto.Points;
                            data.InstRefNo = dataDto.InstRefNo;
                            context.Entry(data).Property(x => x.Name).IsModified = true;
                            context.Entry(data).Property(x => x.IsActive).IsModified = true;
                            context.Entry(data).Property(x => x.Email).IsModified = true;
                            context.Entry(data).Property(x => x.MobileNo).IsModified = true;
                            context.Entry(data).Property(x => x.TelephoneNo).IsModified = true;
                            context.Entry(data).Property(x => x.Address).IsModified = true;
                            context.Entry(data).Property(x => x.Location).IsModified = true;
                            context.Entry(data).Property(x => x.CreatedDate).IsModified = true;
                            context.Entry(data).Property(x => x.RefNo).IsModified = true;
                            context.Entry(data).Property(x => x.Points).IsModified = true;
                            context.Entry(data).Property(x => x.InstRefNo).IsModified = true;                     

                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        Customer data = new Customer();
                        data.Name = dataDto.Name;                       
                        data.Email = dataDto.Email;
                        data.MobileNo = dataDto.MobileNo;
                        data.TelephoneNo = dataDto.TelephoneNo;
                        data.Address = dataDto.Address;
                        data.Location = dataDto.Location;
                        data.CreatedDate = DateTime.Now;
                        data.RefNo = dataDto.RefNo;
                        data.Points = dataDto.Points;
                        data.InstRefNo = dataDto.InstRefNo;
                        data.IsActive = true;
                        context.Customers.Add(data);
                        context.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }


    }
}
