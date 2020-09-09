using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Eahara.Model;
using Eahara.Web.Dtos;
using Eahara.Web.Filter;
using Kendo.DynamicLinq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Eahara.Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MEDBookingController : ApiController
    {
        public static readonly object LockObject = new object();

        [HttpPost]
        [Route("AddMEDBooking")]
        public MEDBookingDto AddMEDBooking(MEDBookingDto dataDto)
        {
            lock (LockObject)
            {
                if (dataDto != null)
                {
                    using (EAharaDB context = new EAharaDB())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                MEDBooking addData = new MEDBooking();
                                var cp = context.CompanyProfiles.FirstOrDefault(x => x.Id > 0);

                                var DateDay = dataDto.Day.ToString();
                                if (DateDay.Length == 1)
                                {
                                    DateDay = "0" + DateDay;
                                }
                                var DateMonth = dataDto.Month.ToString();
                                if (DateMonth.Length == 1)
                                {
                                    DateMonth = "0" + DateMonth;
                                }
                                var DateHour = dataDto.Hour.ToString();
                                if (DateHour.Length == 1)
                                {
                                    DateHour = "0" + DateHour;
                                }
                                var DateMinute = dataDto.Minutes.ToString();
                                if (DateMinute.Length == 1)
                                {
                                    DateMinute = "0" + DateMinute;
                                }
                                var DateYear = dataDto.Year.ToString();
                                var tdate = DateDay + "-" + DateMonth + "-" + DateYear + " " + DateHour + ":" + DateMinute + ":00";
                                DateTime Date = DateTime.ParseExact(tdate, "dd-MM-yyyy HH:mm:ss", null);

                                addData.Description = dataDto.Description;
                                addData.Remarks = dataDto.Remarks;
                                addData.Time = dataDto.Time;
                                addData.Date = DateTime.Now;
                                addData.Count = dataDto.Count;
                                addData.Total = dataDto.Total;
                                addData.Name = dataDto.Name;
                                addData.MobileNo = dataDto.MobileNo;
                                addData.EmailId = dataDto.EmailId;
                                addData.Address = dataDto.Address;
                                addData.Lat = dataDto.Lat;
                                addData.Lng = dataDto.Lng;
                                addData.LocationId = dataDto.LocationId;
                                addData.OrderDate = Date;
                                addData.CustomerId = dataDto.CustomerId;
                                addData.PromoOfferPrice = dataDto.PromoOfferPrice;
                                addData.PromoOfferId = dataDto.PromoOfferId;
                                addData.TotalDeliveryCharge = dataDto.TotalDeliveryCharge;
                                addData.WalletCash = dataDto.WalletCash;
                                addData.SubTotal = dataDto.SubTotal;
                                addData.IsActive = true;
                                addData.IsOrderLater = true;

                                var status = context.MEDStatuses.FirstOrDefault(x => x.Name == "Booked");
                                var location = context.Locations.FirstOrDefault(x => x.Id == dataDto.LocationId);

                                addData.MEDStatusId = status.Id;

                                if (dataDto.Lng != "" && dataDto.Lng != null && dataDto.Lat != "" && dataDto.Lat != null)
                                {
                                    var sCoord = new GeoCoordinate(Convert.ToDouble(location.Lat), Convert.ToDouble(location.Lng));
                                    var eCoord = new GeoCoordinate(Convert.ToDouble(dataDto.Lat), Convert.ToDouble(dataDto.Lng));

                                    var distance = sCoord.GetDistanceTo(eCoord);
                                    var distinKm = distance / 1000;

                                    if (distinKm > location.DeliveryRange)
                                    {
                                        MEDBookingDto retdata2 = new MEDBookingDto();
                                        retdata2.Id = -1;
                                        retdata2.Description = "Sorry ! Cannot Process You Are Far Away From The Delivery Range. Keep Changing Delivery Location.";
                                        return retdata2;
                                    }

                                }

                                var traceNo = context.TraceNoes.FirstOrDefault(x => x.Type == "Booking");
                                if (traceNo == null)
                                {
                                    TraceNo tnmo = new TraceNo();

                                    tnmo.Type = "Booking";
                                    tnmo.Number = 100;
                                    tnmo.Prefix = "BK";

                                    context.TraceNoes.Add(tnmo);
                                    context.SaveChanges();

                                    addData.RefNo = tnmo.Prefix + tnmo.Number;
                                }
                                else
                                {
                                    traceNo.Number = traceNo.Number + 1;
                                    addData.RefNo = traceNo.Prefix + traceNo.Number;

                                    context.Entry(traceNo).Property(x => x.Number).IsModified = true;
                                }

                                if (dataDto.CustomerId > 0)
                                {
                                    var cus = context.Customers.FirstOrDefault(x => x.Id == dataDto.CustomerId);
                                    addData.Name = cus.Name;
                                    addData.MobileNo = cus.MobileNo;
                                    addData.EmailId = cus.Email;

                                    cus.Points = cus.Points - dataDto.WalletCash;
                                    cus.Points = cus.Points + cp.BookingPoints;

                                    context.Entry(cus).Property(x => x.Points).IsModified = true;
                                }
                        
                                addData.MEDBookingDetails = new List<MEDBookingDetail>();
                                foreach (var det in dataDto.MEDBookingDetails)
                                {
                                    MEDBookingDetail Bdetail = new MEDBookingDetail();

                                    Bdetail.MEDBookingId = addData.Id;
                                    Bdetail.Quantity = det.Quantity;
                                    Bdetail.Price = det.Price;
                                    Bdetail.TotalPrice = det.TotalPrice;
                                    Bdetail.MEDItemId = det.MEDItemId;
                                    Bdetail.DelCharge = det.DelCharge;
                                    Bdetail.DiscountPrice = det.DiscountPrice;
                                    Bdetail.IsActive = true;

                                    addData.MEDBookingDetails.Add(Bdetail);

                                    var item = context.MEDItems.FirstOrDefault(x => x.Id == det.MEDItemId);

                                    if (item.MEDShopId > 0)
                                    {
                                        Bdetail.MEDShopId = item.MEDShopId;
                                    }

                                    item.Bookings = item.Bookings + 1;
                                    context.Entry(item).Property(x => x.Bookings).IsModified = true;
                                }

                                MessageController messagectrl = new MessageController();
                                NotificationController Notictrl = new NotificationController();

                                string msg = "Order No " + addData.RefNo + " has been placed by customer " + addData.Name + " Thanks for using HF Services. Issues? Call us on " + cp.MobileNo;
                        
                                Notictrl.addAdminNotification(msg);

                                string msg2 = "Your order No : " + addData.RefNo + " has been Booked, Thanks for using HF Services. Issues ? Call us on " + cp.MobileNo;
                                messagectrl.sendSMS(msg2, addData.MobileNo);

                                if (dataDto.CustomerId > 0)
                                {
                                    Notictrl.addCustomerNotification(msg2, dataDto.CustomerId.Value);
                                }

                                if (dataDto.CustomerId > 0 && dataDto.PromoOfferId > 0)
                                {
                                    CustomerOffer cusoff = new CustomerOffer();
                                    cusoff.CustomerId = dataDto.CustomerId.Value;
                                    cusoff.PromoOfferId = dataDto.PromoOfferId.Value;
                                    cusoff.RefNo = addData.RefNo;
                                    context.CustomerOffers.Add(cusoff);
                                }

                                if (dataDto.AddressId <= 0 && dataDto.CustomerId != null)
                                {
                                    Address add = new Address();
                                    add.Description = dataDto.Address;
                                    add.Lat = dataDto.Lat;
                                    add.Lng = dataDto.Lng;
                                    add.CustomerId = dataDto.CustomerId.Value;
                                    context.Addresses.Add(add);
                                }

                                context.MEDBookings.Add(addData);
                                context.SaveChanges();

                                if (dataDto.MEDUploadId > 0)
                                {
                                    var medupload = context.MEDUploads.FirstOrDefault(x => x.Id == dataDto.MEDUploadId);
                                    if (medupload != null)
                                    {
                                        medupload.MEDBookingId = addData.Id;
                                        context.Entry(medupload).Property(x => x.MEDBookingId).IsModified = true;
                                        context.SaveChanges();
                                    }
                                }

                                transaction.Commit();

                                MEDBookingDto retdata = new MEDBookingDto();
                                retdata.Id = addData.Id;
                                retdata.RefNo = addData.RefNo;

                              
                                return retdata;
                            }
                            catch (Exception e)
                            {
                                transaction.Rollback();
                                return null;
                            }
                        }
                    }
                }
                return null;
            }
        }

        [HttpGet]
        [Route("DeleteMEDBooking/{id}")]
        public bool DeleteMEDBookingById(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.MEDBookings.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {
                        Delete.IsActive = false;
                        context.Entry(Delete).Property(x => x.IsActive).IsModified = true;

                        NotificationController Notictrl = new NotificationController();
                        Notictrl.addAdminNotification("MEDBooking Deleted : " + Delete.RefNo);
                    }
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }

        [OrangeAuthorizationFilter]
        [HttpPost]
        [Route("MEDBookingsInView")]
        public DataSourceResult MEDBookingsInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {

                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;

                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var query = context.MEDBookings.Where(x => x.IsActive == true);

                if (user.Role != "Admin" && user.Role != "admin" && user.Role != "0")
                {
                    if (user.Employee != null)
                    {
                        query = query.Where(x => x.LocationId == user.Employee.LocationId);
                    }
                }

                var dataSourceResult = query
                    .Select(x => new MEDBookingDto
                    {

                        Id = x.Id,
                        Description = x.Description,
                        Remarks = x.Remarks,
                        Time = x.Time,
                        PickUpDate = x.PickUpDate,
                        AssignedDate = x.AssignedDate,
                        StatusDate = x.StatusDate,
                        Date = x.Date,
                        Count = x.Count,
                        Total = x.Total,
                        Name = x.Name,
                        MobileNo = x.MobileNo,
                        EmailId = x.EmailId,
                        IsPaid = x.IsPaid,
                        Address = x.Address,
                        Lat = x.Lat,
                        Lng = x.Lng,
                        OrderDate = x.OrderDate,
                        RefNo = x.RefNo,
                        PromoOfferPrice = x.PromoOfferPrice,
                        WalletCash = x.WalletCash,
                        MEDStatusId = x.MEDStatusId,
                        MEDStatus = new MEDStatusDto
                        {
                            Id = x.MEDStatus != null ? x.MEDStatus.Id : 0,
                            Name = x.MEDStatus != null ? x.MEDStatus.Name : "",
                        },
                        EmployeeId = x.EmployeeId,
                        Employee = new EmployeeDto
                        {
                            Id = x.Employee != null ? x.Employee.Id : 0,
                            Name = x.Employee != null ? x.Employee.Name : "",
                        },

                        SubTotal = x.SubTotal,
                        TotalDeliveryCharge = x.TotalDeliveryCharge,
                        LocationId = x.LocationId,
                        Location = new LocationDto
                        {
                            Id = x.Location != null ? x.Location.Id : 0,
                            Name = x.Location != null ? x.Location.Name : "",
                        },

                        
                    }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);
                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }

        [HttpGet]
        [Route("MEDBookingDetailsById/{id}")]
        public MEDBookingDto MEDBookingDetailsById(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDBookings.FirstOrDefault(x => x.IsActive == true && x.Id == id);

                MEDBookingDto bkdto = new MEDBookingDto();

                bkdto.Id = data.Id;
                bkdto.Description = data.Description;
                bkdto.Remarks = data.Remarks;
                bkdto.Time = data.Time;
                bkdto.Date = data.Date;
                bkdto.Count = data.Count;
                bkdto.Total = data.Total;
                bkdto.Name = data.Name;
                bkdto.IsPaid = data.IsPaid;
                bkdto.MobileNo = data.MobileNo;
                bkdto.EmailId = data.EmailId;
                bkdto.PickUpDate = data.PickUpDate;
                bkdto.StatusDate = data.StatusDate;
                bkdto.AssignedDate = data.AssignedDate;
                bkdto.Address = data.Address;
                bkdto.Lat = data.Lat;
                bkdto.Lng = data.Lng;
                bkdto.OrderDate = data.OrderDate;
                bkdto.RefNo = data.RefNo;
                bkdto.LocationId = data.LocationId;
                bkdto.PromoOfferPrice = data.PromoOfferPrice;
                bkdto.WalletCash = data.WalletCash;
                bkdto.EmployeeId = data.EmployeeId;
                bkdto.Employee = new EmployeeDto
                {
                    Id = data.Employee != null ? data.Employee.Id : 0,
                    Name = data.Employee != null ? data.Employee.Name : "",
                };
                bkdto.MEDStatusId = data.MEDStatusId;
                bkdto.MEDStatus = new MEDStatusDto
                {
                    Id = data.MEDStatus != null ? data.MEDStatus.Id : 0,
                    Name = data.MEDStatus != null ? data.MEDStatus.Name : "",
                };                
                bkdto.SubTotal = data.SubTotal;
                bkdto.TotalDeliveryCharge = data.TotalDeliveryCharge;
                bkdto.Location = new LocationDto
                {
                    Id = data.Location != null ? data.Location.Id : 0,
                    Name = data.Location != null ? data.Location.Name : "",
                };
                bkdto.PromoOffer = new PromoOfferDto
                {
                    Id = data.PromoOffer != null ? data.PromoOffer.Id : 0,
                    Tittle = data.PromoOffer != null ? data.PromoOffer.Tittle : "",
                };

                bkdto.MEDBookingDetails = context.MEDBookingDetails.Where(y => y.IsActive == true && y.MEDBookingId == data.Id)
                                .Select(y => new MEDBookingDetailDto
                                {
                                    Id = y.Id,
                                    Quantity = y.Quantity,
                                    Price = y.Price,
                                    TotalPrice = y.TotalPrice,
                                    DiscountPrice = y.DiscountPrice,
                                    DelCharge = y.DelCharge,
                                    Remarks = y.Remarks,
                                    MEDItemId = y.MEDItemId,
                                    MEDItem = new MEDItemDto
                                    {
                                        Id  = y.MEDItem.Id,
                                        Name = y.MEDItem.Name,
                                        Price = y.MEDItem.Price,
                                        OfferPrice = y.MEDItem.OfferPrice,                                        
                                    },
                                    IsActive = true,                                                                       

                                }).ToList();

                return bkdto;
            }
        }

        [HttpPost]
        [Route("UpdateMEDBookingStatus")]
        public bool UpdateMEDBookingStatus(FilterDto dataDto)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDBookings.FirstOrDefault(x => x.Id == dataDto.id);
                var cp = context.CompanyProfiles.FirstOrDefault(x => x.Id > 0);
                if (data != null)
                {
                    data.MEDStatusId = dataDto.sid;
                    context.Entry(data).Property(x => x.MEDStatusId).IsModified = true;
                    data.CancelRemarks = dataDto.CancelRemarks;
                    context.Entry(data).Property(x => x.CancelRemarks).IsModified = true;

                    data.StatusDate = DateTime.Now;
                    context.Entry(data).Property(x => x.StatusDate).IsModified = true;

                    context.SaveChanges();

                    var status = context.MEDStatuses.FirstOrDefault(x => x.Id == dataDto.sid);
                    //if (status.Name == "Delivered")
                    //{
                    //    MessageController messagectrl = new MessageController();

                    //    string msg = "You order No "+ data.Booking.RefNo + " has been delivered, thanks for using Eahara, Issues ? Call us on Number " + cp.MobileNo + "  or " + cp.WhatsappNo; 

                    //    messagectrl.sendSMS(msg, data.Booking.MobileNo);
                    //}

                    NotificationController Notictrl = new NotificationController();
                    Notictrl.addAdminNotification("Booking Status Changed : " + data.RefNo + " to : " + status.Name);                   

                    if (status.Name == "Approved")
                    {
                        MessageController messagectrl = new MessageController();

                        string msg = "Your Order " + data.RefNo
                            + " is Approved. Our executive will deliver it soon, thanks for using HF Services. Issues ? " +
                            "Call us on Number " + cp.MobileNo;

                        messagectrl.sendSMS(msg, data.MobileNo);
                        if (data.CustomerId > 0)
                        {
                            Notictrl.addCustomerNotification(msg, data.CustomerId.Value);
                        }
                    }
                    else if (status.Name == "Delivered" || status.Name == "Cancelled")
                    {
                        MessageController messagectrl = new MessageController();

                        string msg = "Your order no " + data.RefNo + " has been " + status.Name + " ," +
                            " thanks for using HF Services, Issues ? Call us on Number " + cp.MobileNo;

                        messagectrl.sendSMS(msg, data.MobileNo);
                        if (data.CustomerId > 0)
                        {
                            Notictrl.addCustomerNotification(msg, data.CustomerId.Value);
                        }
                    }


                    return true;
                }
                return true;
            }
        }


        [HttpPost]
        [Route("ApplyMEDRemarksAdmin")]
        public bool ApplyMEDRemarksAdmin(FilterDto dataDto)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDBookings.FirstOrDefault(x => x.Id == dataDto.id);
                if (data != null)
                {
                    data.Remarks = dataDto.Remarks;
                    context.Entry(data).Property(x => x.Remarks).IsModified = true;
                    context.SaveChanges();

                    return true;
                }
                return true;
            }
        }

        [HttpGet]
        [Route("MakePaidMEDStatusBooking/{id}")]
        public bool MakePaidMEDStatusBooking(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.MEDBookings.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {
                        if (Delete.IsPaid)
                        {
                            Delete.IsPaid = false;
                            context.Entry(Delete).Property(x => x.IsPaid).IsModified = true;
                        }
                        else
                        {
                            Delete.IsPaid = true;
                            context.Entry(Delete).Property(x => x.IsPaid).IsModified = true;
                        }
                        NotificationController Notictrl = new NotificationController();
                        Notictrl.addAdminNotification("Booking Paid Status Changed : " + Delete.RefNo);

                    }
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }

        [HttpGet]
        [Route("ApplyDelMEDChargeAdmin/{id}/{value}")]
        public bool ApplyDelMEDChargeAdmin(long id, float value)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.MEDBookings.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {
                        Delete.TotalDeliveryCharge = value;
                        Delete.Total = (Delete.TotalDeliveryCharge + Delete.SubTotal) - Delete.WalletCash;

                        context.Entry(Delete).Property(x => x.TotalDeliveryCharge).IsModified = true;
                        context.Entry(Delete).Property(x => x.Total).IsModified = true;

                        NotificationController Notictrl = new NotificationController();
                        Notictrl.addAdminNotification("Booking Paid Status Changed : " + Delete.RefNo);

                    }
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }

        [HttpGet]
        [Route("ApplyMEDPromoOfferAdmin/{id}/{value}")]
        public bool ApplyMEDPromoOfferAdmin(long id, float value)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.MEDBookings.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {
                        Delete.SubTotal = (Delete.SubTotal + Delete.PromoOfferPrice) - value;
                        Delete.PromoOfferPrice = value;
                        Delete.Total = (Delete.TotalDeliveryCharge + Delete.SubTotal) - Delete.WalletCash;

                        context.Entry(Delete).Property(x => x.SubTotal).IsModified = true;
                        context.Entry(Delete).Property(x => x.PromoOfferPrice).IsModified = true;
                        context.Entry(Delete).Property(x => x.Total).IsModified = true;

                        NotificationController Notictrl = new NotificationController();
                        Notictrl.addAdminNotification("Booking Paid Status Changed : " + Delete.RefNo);

                    }
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }


        [HttpPost]
        [Route("MEDBookingReportsInView")]
        public DataSourceResult MEDBookingReportsInView(KendoFilterRequestDto Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var rep = context.MEDBookings.Where(x => x.IsActive &&
                                               ((DbFunctions.TruncateTime(x.OrderDate)) >= (DbFunctions.TruncateTime(Request.FromDate))
                                              && (DbFunctions.TruncateTime(x.OrderDate)) <= (DbFunctions.TruncateTime(Request.ToDate))));

              
                if (Request.StatusId > 0)
                {
                    rep = rep.Where(x => x.MEDStatusId == Request.StatusId);
                }

                if (Request.LocationId > 0)
                {
                    rep = rep.Where(x => x.LocationId == Request.LocationId);
                }

                if (Request.Paid == "Paid")
                {
                    rep = rep.Where(x => x.IsPaid);
                }
                if (Request.Paid == "Not Paid")
                {
                    rep = rep.Where(x => !x.IsPaid);
                }

                var dataSourceResult = rep.Select(x => new MEDBookingDto

                {

                    Id = x.Id,
                    Description = x.Description,
                    Remarks = x.Remarks,
                    Time = x.Time,
                    Date = x.Date,
                    Count = x.Count,
                    Total = x.Total,
                    Name = x.Name,
                    MobileNo = x.MobileNo,
                    EmailId = x.EmailId,
                    Address = x.Address,
                    PickUpDate = x.PickUpDate,
                    AssignedDate = x.AssignedDate,
                    StatusDate = x.StatusDate,
                    Lat = x.Lat,
                    Lng = x.Lng,
                    OrderDate = x.OrderDate,
                    IsPaid = x.IsPaid,
                    RefNo = x.RefNo,
                    PromoOfferPrice = x.PromoOfferPrice,
                    WalletCash = x.WalletCash,
                    MEDStatusId = x.MEDStatusId,
                    MEDStatus = new MEDStatusDto
                    {
                        Id = x.MEDStatus != null ? x.MEDStatus.Id : 0,
                        Name = x.MEDStatus != null ? x.MEDStatus.Name : "",
                    },
                    EmployeeId = x.EmployeeId,
                    Employee = new EmployeeDto
                    {
                        Id = x.Employee != null ? x.Employee.Id : 0,
                        Name = x.Employee != null ? x.Employee.Name : "",
                    },                    
                    SubTotal = x.SubTotal,
                    TotalDeliveryCharge = x.TotalDeliveryCharge,
                    LocationId = x.LocationId,
                    Location = new LocationDto
                    {
                        Id = x.Location != null ? x.Location.Id : 0,
                        Name = x.Location != null ? x.Location.Name : "",
                    },
                }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }

        [HttpPost]
        [Route("MEDUploadedListInView")]
        public DataSourceResult MEDUploadedListInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.MEDBookings.Where(x => x.IsActive == true)
                    .Select(x => new MEDBookingDto
                    {

                        Id = x.Id,
                        Description = x.Description,
                        Remarks = x.Remarks,
                        Time = x.Time,
                        Date = x.Date,
                        Count = x.Count,
                        Total = x.Total,
                        Name = x.Name,
                        MobileNo = x.MobileNo,
                        EmailId = x.EmailId,
                        IsPaid = x.IsPaid,
                        Address = x.Address,
                        Lat = x.Lat,
                        PickUpDate = x.PickUpDate,
                        AssignedDate = x.AssignedDate,
                        StatusDate = x.StatusDate,
                        Lng = x.Lng,
                        OrderDate = x.OrderDate,
                        RefNo = x.RefNo,
                        PromoOfferPrice = x.PromoOfferPrice,
                        WalletCash = x.WalletCash,
                        MEDStatusId = x.MEDStatusId,
                        MEDStatus = new MEDStatusDto
                        {
                            Id = x.MEDStatus != null ? x.MEDStatus.Id : 0,
                            Name = x.MEDStatus != null ? x.MEDStatus.Name : "",
                        },
                        EmployeeId = x.EmployeeId,
                        Employee = new EmployeeDto
                        {
                            Id = x.Employee != null ? x.Employee.Id : 0,
                            Name = x.Employee != null ? x.Employee.Name : "",
                        },

                        SubTotal = x.SubTotal,
                        TotalDeliveryCharge = x.TotalDeliveryCharge,
                        LocationId = x.LocationId,
                        Location = new LocationDto
                        {
                            Id = x.Location != null ? x.Location.Id : 0,
                            Name = x.Location != null ? x.Location.Name : "",
                        },




                    }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }

        [HttpGet]
        [Route("MyMEDOrdersInApp/{id}")]
        public List<MEDBookingDto> MyMEDOrdersInApp(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.MEDBookings.Where(x => x.IsActive == true && x.CustomerId == id)
                    .Select(x => new MEDBookingDto
                    {

                        Id = x.Id,
                        Description = x.Description,
                        Remarks = x.Remarks,
                        Time = x.Time,
                        PickUpDate = x.PickUpDate,
                        AssignedDate = x.AssignedDate,
                        StatusDate = x.StatusDate,
                        Date = x.Date,
                        Count = x.Count,
                        IsPaid = x.IsPaid,
                        Total = x.Total,
                        Name = x.Name,
                        MobileNo = x.MobileNo,
                        EmailId = x.EmailId,
                        Address = x.Address,
                        PromoOfferPrice = x.PromoOfferPrice,
                        CancelRemarks = x.CancelRemarks,
                        WalletCash = x.WalletCash,
                        Lat = x.Lat,
                        Lng = x.Lng,
                        OrderDate = x.OrderDate,
                        RefNo = x.RefNo,
                        EmployeeId = x.EmployeeId,
                        Employee = new EmployeeDto
                        {
                            Id = x.Employee != null ? x.Employee.Id : 0,
                            Name = x.Employee != null ? x.Employee.Name : "",
                        },
                        LocationId = x.LocationId,
                        Location = new LocationDto
                        {
                            Id = x.Location != null ? x.Location.Id : 0,
                            Name = x.Location != null ? x.Location.Name : "",
                        },
                        PromoOffer = new PromoOfferDto
                        {
                            Id = x.PromoOffer != null ? x.PromoOffer.Id : 0,
                            Tittle = x.PromoOffer != null ? x.PromoOffer.Tittle : "",
                        },
                        MEDStatusId = x.MEDStatusId,
                        StatusName = x.MEDStatus != null ? x.MEDStatus.Name : "",
                        SubTotal = x.SubTotal,
                        TotalDeliveryCharge = x.TotalDeliveryCharge,
                        MEDBookingDetails = x.MEDBookingDetails.Where(y => y.IsActive)
                        .Select(y => new MEDBookingDetailDto
                        {
                            Id = y.Id,
                            Quantity = y.Quantity,
                            Price = y.Price,
                            TotalPrice = y.TotalPrice,
                            DiscountPrice = y.DiscountPrice,
                            DelCharge = y.DelCharge,
                            MEDItemId = y.MEDItemId,
                            Remarks = y.Remarks,
                            IsActive = true,
                            MEDItem = new MEDItemDto
                            {
                                Name = y.MEDItem != null ? y.MEDItem.Name : "",
                                Id = y.MEDItem != null ? y.MEDItem.Id : 0,
                            },
                        }).ToList(),

                    }).OrderByDescending(x => x.Id).ToList();

                return dataSourceResult;
            }
        }

        [HttpPost]
        [Route("CancelMEDBooking")]
        public bool CancelMEDBooking(FilterDto dataDto)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDBookings.FirstOrDefault(x => x.Id == dataDto.id);
                var status = context.MEDStatuses.FirstOrDefault(x => x.Name == "Cancelled");
                if (data != null)
                {
                    if (status != null)
                    {
                        data.MEDStatusId = status.Id;
                        context.Entry(data).Property(x => x.MEDStatusId).IsModified = true;
                        data.CancelRemarks = dataDto.CancelRemarks;
                        context.Entry(data).Property(x => x.Remarks).IsModified = true;

                        data.StatusDate = DateTime.Now;
                        context.Entry(data).Property(x => x.StatusDate).IsModified = true;

                        context.SaveChanges();

                        NotificationController Notictrl = new NotificationController();
                        Notictrl.addAdminNotification("Booking Cancelled : " + data.RefNo);
                        if (data.CustomerId > 0)
                        {
                            Notictrl.addCustomerNotification("Booking Cancelled : " + data.RefNo, data.CustomerId.Value);
                        }


                        return true;
                    }
                }
                return true;
            }
        }
        
        [HttpGet]
        [Route("TrackMEDOrder/{id}")]
        public MEDBookingDto TrackMEDOrder(string id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDBookings.Where(x => x.IsActive == true && x.RefNo == id)
                  .Select(x => new MEDBookingDto
                  {
                      Id = x.Id,
                      Description = x.Description,
                      Remarks = x.Remarks,
                      Time = x.Time,
                      Date = x.Date,
                      Count = x.Count,
                      PickUpDate = x.PickUpDate,
                      AssignedDate = x.AssignedDate,
                      StatusDate = x.StatusDate,
                      Total = x.Total,
                      Name = x.Name,
                      MobileNo = x.MobileNo,
                      EmailId = x.EmailId,
                      Address = x.Address,
                      PromoOfferPrice = x.PromoOfferPrice,
                      Lat = x.Lat,
                      Lng = x.Lng,
                      IsPaid = x.IsPaid,
                      OrderDate = x.OrderDate,
                      WalletCash = x.WalletCash,
                      RefNo = x.RefNo,
                      EmployeeId = x.EmployeeId,
                      Employee = new EmployeeDto
                      {
                          Id = x.Employee != null ? x.Employee.Id : 0,
                          Name = x.Employee != null ? x.Employee.Name : "",
                      },
                      LocationId = x.LocationId,
                      Location = new LocationDto
                      {
                          Id = x.Location != null ? x.Location.Id : 0,
                          Name = x.Location != null ? x.Location.Name : "",
                      },
                      MEDStatusId = x.MEDStatusId,
                      StatusName = x.MEDStatus != null ? x.MEDStatus.Name : "",
                      SubTotal = x.SubTotal,
                      TotalDeliveryCharge = x.TotalDeliveryCharge,
                      MEDBookingDetails = x.MEDBookingDetails.Where(y => y.IsActive)
                      .Select(y => new MEDBookingDetailDto
                      {
                          Id = y.Id,
                          Quantity = y.Quantity,
                          Price = y.Price,
                          TotalPrice = y.TotalPrice,
                          DiscountPrice = y.DiscountPrice,
                          DelCharge = y.DelCharge,
                          MEDItemId = y.MEDItemId,
                          Remarks = y.Remarks,
                          IsActive = true,
                          MEDItem = new MEDItemDto
                          {
                              Name = y.MEDItem != null ? y.MEDItem.Name : "",
                              Id = y.MEDItem != null ? y.MEDItem.Id : 0,
                          },
                      }).ToList(),

                  }).FirstOrDefault();


                return data;
            }
        }

        [HttpPost]
        [Route("PrintMedBookingReportList")]
        public string PrintMedBookingReportList(KendoFilterRequestDto Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var rep = context.MEDBookings.Where(x => x.IsActive &&
                                               ((DbFunctions.TruncateTime(x.OrderDate)) >= (DbFunctions.TruncateTime(Request.FromDate))
                                              && (DbFunctions.TruncateTime(x.OrderDate)) <= (DbFunctions.TruncateTime(Request.ToDate))));

                if (Request.Paid == "Paid")
                {
                    rep = rep.Where(x => x.IsPaid);
                }
                if (Request.Paid == "Not Paid")
                {
                    rep = rep.Where(x => !x.IsPaid);
                }

                long[] items = rep.Select(x => x.Id).ToArray();

                ReportDocument rd = new ReportDocument();

                Guid id1 = Guid.NewGuid();
                var pdfName = "MEDBookingReport" + id1 + ".pdf";
                string strRptPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Reports\\" + "MEDBookingReport" + ".rpt";
                string strPdfPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Reports\\" + pdfName;

                rd.Load(strRptPath);
                rd.Refresh();

                string connectionString =
                ConfigurationManager.ConnectionStrings["EAharaDB"].ConnectionString;

                SqlConnectionStringBuilder SConn = new SqlConnectionStringBuilder(connectionString);

                rd.DataSourceConnections[0].SetConnection(
                  SConn.DataSource, SConn.InitialCatalog, SConn.UserID, SConn.Password);

                foreach (ReportDocument srd in rd.Subreports)
                {
                    srd.DataSourceConnections[0].SetConnection(SConn.DataSource, SConn.InitialCatalog, SConn.UserID, SConn.Password);
                }
                rd.SetParameterValue(0, items);
                System.IO.File.Delete(strPdfPath);
                rd.PrintOptions.PaperSize = PaperSize.PaperA4;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, strPdfPath);

                return pdfName;
            }

        }
        
        [HttpGet]
        [Route("AssignMEDDriver/{id}/{DriverId}")]
        public bool AssignMEDDriver(long id, long DriverId)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.MEDBookings.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {
                        Delete.EmployeeId = DriverId;
                        context.Entry(Delete).Property(x => x.EmployeeId).IsModified = true;

                        Delete.AssignedDate = DateTime.Now;
                        context.Entry(Delete).Property(x => x.AssignedDate).IsModified = true;

                        var user = context.Users.FirstOrDefault(x => x.IsActive && x.EmployeeId == DriverId);
                        if (user != null)
                        {
                            NotificationController Notictrl = new NotificationController();
                            var msg = "Booking Assigned : " + Delete.RefNo;
                            Notictrl.addEmployeeNotification(msg, user.Id);
                        }
                    }
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }
               
        [HttpPost]
        [Route("MEDBookingReportsShop")]
        public List<MEDBookingDto> MEDBookingReportsShop(KendoFilterRequestDto Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var shop = context.MEDShops.FirstOrDefault(x => x.Id == Request.MEDShopId);
                var rep = context.MEDBookings.Where(x => x.IsActive &&
                                               ((DbFunctions.TruncateTime(x.OrderDate)) >= (DbFunctions.TruncateTime(Request.FromDate))
                                              && (DbFunctions.TruncateTime(x.OrderDate)) <= (DbFunctions.TruncateTime(Request.ToDate))));

                if (Request.MEDShopId > 0)
                {
                    rep = rep.Where(x => x.MEDBookingDetails.Where(y=>y.MEDShopId == Request.MEDShopId).Count() >0);
                }

                if (Request.Paid == "Paid")
                {
                    rep = rep.Where(x => x.IsPaid);
                }
                if (Request.Paid == "Not Paid")
                {
                    rep = rep.Where(x => !x.IsPaid);
                }

                var dataSourceResult = rep.Select(x => new MEDBookingDto
                {

                    Id = x.Id,
                    Description = x.Description,
                    Remarks = x.Remarks,
                    Time = x.Time,
                    Date = x.Date,
                    Count = x.Count,
                    Total = x.Total,
                    Name = x.Name,
                    PickUpDate = x.PickUpDate,
                    AssignedDate = x.AssignedDate,
                    StatusDate = x.StatusDate,
                    MobileNo = x.MobileNo,
                    EmailId = x.EmailId,
                    Address = x.Address,
                    Lat = x.Lat,
                    Lng = x.Lng,
                    OrderDate = x.OrderDate,
                    IsPaid = x.IsPaid,
                    RefNo = x.RefNo,
                    PromoOfferPrice = x.PromoOfferPrice,
                    WalletCash = x.WalletCash,
                    ActualTotal = x.MEDBookingDetails.Where(y=>y.IsActive && y.MEDShopId == Request.MEDShopId).Sum(y=>y.TotalPrice),
                    Commission = (x.MEDBookingDetails.Where(y => y.IsActive && y.MEDShopId == Request.MEDShopId).Sum(y => y.TotalPrice) * shop.CommissionPercentage) / 100,
                    MEDStatusId = x.MEDStatusId,
                    EmployeeId = x.EmployeeId,
                    Employee = new EmployeeDto
                    {
                        Id = x.Employee != null ? x.Employee.Id : 0,
                        Name = x.Employee != null ? x.Employee.Name : "",
                    },
                    MEDStatus = new MEDStatusDto
                    {
                        Id = x.MEDStatus != null ? x.MEDStatus.Id : 0,
                        Name = x.MEDStatus != null ? x.MEDStatus.Name : "",
                    },                   
                    SubTotal = x.SubTotal,
                    TotalDeliveryCharge = x.TotalDeliveryCharge,
                    LocationId = x.LocationId,
                    Location = new LocationDto
                    {
                        Id = x.Location != null ? x.Location.Id : 0,
                        Name = x.Location != null ? x.Location.Name : "",
                    },
                    MEDBookingDetails = x.MEDBookingDetails.Where(y => y.IsActive && y.MEDShopId == Request.MEDShopId)
                      .Select(y => new MEDBookingDetailDto
                      {
                          Id = y.Id,
                          Quantity = y.Quantity,
                          Price = y.Price,
                          TotalPrice = y.TotalPrice,
                          DiscountPrice = y.DiscountPrice,
                          DelCharge = y.DelCharge,
                          MEDItemId = y.MEDItemId,
                          MEDShopId = y.MEDShopId,
                          Remarks = y.Remarks,
                          IsActive = true,
                          MEDItem = new MEDItemDto
                          {
                              Name = y.MEDItem != null ? y.MEDItem.Name : "",
                              Id = y.MEDItem != null ? y.MEDItem.Id : 0,
                          },
                      }).ToList(),
                }).OrderByDescending(x => x.Id).ToList();


                return dataSourceResult;
            }
        }

        [HttpGet]
        [Route("MEDDashBoardCounts/{id}")]
        public FilterDto MEDDashBoardCounts(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                FilterDto filterdto = new FilterDto();

                filterdto.Cancelled = context.MEDBookings.Where(x => (DbFunctions.TruncateTime(x.OrderDate)) == (DbFunctions.TruncateTime(DateTime.Now)) && x.MEDBookingDetails.Where(y=>y.IsActive && y.MEDShopId == id).Count() > 0 && x.MEDStatus.Name == "Cancelled").Count();
                filterdto.Delivered = context.MEDBookings.Where(x => (DbFunctions.TruncateTime(x.OrderDate)) == (DbFunctions.TruncateTime(DateTime.Now)) && x.MEDBookingDetails.Where(y => y.IsActive && y.MEDShopId == id).Count() > 0 && x.MEDStatus.Name == "Delivered").Count();
                filterdto.Approved = context.MEDBookings.Where(x => (DbFunctions.TruncateTime(x.OrderDate)) == (DbFunctions.TruncateTime(DateTime.Now)) && x.MEDBookingDetails.Where(y => y.IsActive && y.MEDShopId == id).Count() > 0 && x.MEDStatus.Name == "Approved").Count();
                filterdto.Packed = context.MEDBookings.Where(x => (DbFunctions.TruncateTime(x.OrderDate)) == (DbFunctions.TruncateTime(DateTime.Now)) && x.MEDBookingDetails.Where(y => y.IsActive && y.MEDShopId == id).Count() > 0 && x.MEDStatus.Name == "Packed").Count();
                filterdto.New = context.MEDBookings.Where(x => (DbFunctions.TruncateTime(x.OrderDate)) == (DbFunctions.TruncateTime(DateTime.Now)) && x.MEDBookingDetails.Where(y => y.IsActive && y.MEDShopId == id).Count() > 0 && x.MEDStatus.Name == "Booked").Count();

                return filterdto;
            }
        }

        [OrangeAuthorizationFilter]
        [HttpGet]
        [Route("BookedMEDBookingsInDashboard/{id}")]
        public List<MEDBookingDto> BookedMEDBookingsInDashboard(int id)
        {
            using (EAharaDB context = new EAharaDB())
            {

                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;

                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var query = context.MEDBookings.Where(x => x.IsActive && x.MEDStatus.Name == "Booked");

                if (user.Role != "Admin" && user.Role != "admin" && user.Role != "0")
                {
                    if (user.Employee != null)
                    {
                        query = query.Where(x => x.LocationId == user.Employee.LocationId);
                    }
                }


                var data = query
                    .Select(x => new MEDBookingDto
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Remarks = x.Remarks,
                        Time = x.Time,
                        Date = x.Date,
                        Count = x.Count,
                        Total = x.Total,
                        Name = x.Name,
                        MobileNo = x.MobileNo,
                        EmailId = x.EmailId,
                        Address = x.Address,
                        PickUpDate = x.PickUpDate,
                        AssignedDate = x.AssignedDate,
                        StatusDate = x.StatusDate,
                        Lat = x.Lat,
                        Lng = x.Lng,
                        OrderDate = x.OrderDate,
                        IsPaid = x.IsPaid,
                        PromoOfferPrice = x.PromoOfferPrice,
                        WalletCash = x.WalletCash,
                        RefNo = x.RefNo,
                        MEDStatusId = x.MEDStatusId,
                        StatusName = x.MEDStatus != null ? x.MEDStatus.Name : "",
                        EmployeeId = x.EmployeeId,
                        Employee = new EmployeeDto
                        {
                            Id = x.Employee != null ? x.Employee.Id : 0,
                            Name = x.Employee != null ? x.Employee.Name : "",
                        },
                        LocationId = x.LocationId,
                        Location = new LocationDto
                        {
                            Id = x.Location != null ? x.Location.Id : 0,
                            Name = x.Location != null ? x.Location.Name : "",
                        },
                        SubTotal = x.SubTotal,
                        TotalDeliveryCharge = x.TotalDeliveryCharge,
                    }).OrderByDescending(x => x.Id).Skip(id).Take(10).ToList();

                return data;
            }
        }

        [OrangeAuthorizationFilter]
        [HttpGet]
        [Route("ApprovedMEDBookingsInDashboard/{id}")]
        public List<MEDBookingDto> ApprovedMEDBookingsInDashboard(int id)
        {
            using (EAharaDB context = new EAharaDB())
            {

                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;

                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var query = context.MEDBookings.Where(x => x.IsActive && x.MEDStatus.Name == "Approved");

                if (user.Role != "Admin" && user.Role != "admin" && user.Role != "0")
                {
                    if (user.Employee != null)
                    {
                        query = query.Where(x => x.LocationId == user.Employee.LocationId);
                    }
                }

                var data = query
                    .Select(x => new MEDBookingDto
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Remarks = x.Remarks,
                        Time = x.Time,
                        Date = x.Date,
                        Count = x.Count,
                        Total = x.Total,
                        PickUpDate = x.PickUpDate,
                        AssignedDate = x.AssignedDate,
                        StatusDate = x.StatusDate,
                        Name = x.Name,
                        MobileNo = x.MobileNo,
                        EmailId = x.EmailId,
                        Address = x.Address,
                        Lat = x.Lat,
                        Lng = x.Lng,
                        OrderDate = x.OrderDate,
                        IsPaid = x.IsPaid,
                        PromoOfferPrice = x.PromoOfferPrice,
                        WalletCash = x.WalletCash,
                        RefNo = x.RefNo,
                        EmployeeId = x.EmployeeId,
                        Employee = new EmployeeDto
                        {
                            Id = x.Employee != null ? x.Employee.Id : 0,
                            Name = x.Employee != null ? x.Employee.Name : "",
                        },
                        MEDStatusId = x.MEDStatusId,
                        StatusName = x.MEDStatus != null ? x.MEDStatus.Name : "",
                        LocationId = x.LocationId,
                        Location = new LocationDto
                        {
                            Id = x.Location != null ? x.Location.Id : 0,
                            Name = x.Location != null ? x.Location.Name : "",
                        },
                        SubTotal = x.SubTotal,
                        TotalDeliveryCharge = x.TotalDeliveryCharge,
                    }).OrderByDescending(x => x.Id).Skip(id).Take(10).ToList();

                return data;
            }
        }

        [OrangeAuthorizationFilter]
        [HttpGet]
        [Route("ReadyMEDBookingsInDashboard/{id}")]
        public List<MEDBookingDto> ReadyMEDBookingsInDashboard(int id)
        {
            using (EAharaDB context = new EAharaDB())
            {

                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;

                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var query = context.MEDBookings.Where(x => x.IsActive && x.MEDStatus.Name == "Ready");

                if (user.Role != "Admin" && user.Role != "admin" && user.Role != "0")
                {
                    if (user.Employee != null)
                    {
                        query = query.Where(x => x.LocationId == user.Employee.LocationId);
                    }
                }


                var data = query
                    .Select(x => new MEDBookingDto
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Remarks = x.Remarks,
                        Time = x.Time,
                        Date = x.Date,
                        PickUpDate = x.PickUpDate,
                        AssignedDate = x.AssignedDate,
                        StatusDate = x.StatusDate,
                        Count = x.Count,
                        Total = x.Total,
                        Name = x.Name,
                        MobileNo = x.MobileNo,
                        EmailId = x.EmailId,
                        Address = x.Address,
                        Lat = x.Lat,
                        Lng = x.Lng,
                        EmployeeId = x.EmployeeId,
                        Employee = new EmployeeDto
                        {
                            Id = x.Employee != null ? x.Employee.Id : 0,
                            Name = x.Employee != null ? x.Employee.Name : "",
                        },
                        OrderDate = x.OrderDate,
                        IsPaid = x.IsPaid,
                        PromoOfferPrice = x.PromoOfferPrice,
                        WalletCash = x.WalletCash,
                        RefNo = x.RefNo,
                        MEDStatusId = x.MEDStatusId,
                        StatusName = x.MEDStatus != null ? x.MEDStatus.Name : "",
                        LocationId = x.LocationId,
                        Location = new LocationDto
                        {
                            Id = x.Location != null ? x.Location.Id : 0,
                            Name = x.Location != null ? x.Location.Name : "",
                        },
                        SubTotal = x.SubTotal,
                        TotalDeliveryCharge = x.TotalDeliveryCharge,
                    }).OrderByDescending(x => x.Id).Skip(id).Take(10).ToList();

                return data;
            }
        }

        [OrangeAuthorizationFilter]
        [HttpGet]
        [Route("PickedMEDBookingsInDashboard/{id}")]
        public List<MEDBookingDto> PickedMEDBookingsInDashboard(int id)
        {
            using (EAharaDB context = new EAharaDB())
            {

                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;

                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var query = context.MEDBookings.Where(x => x.IsActive && x.MEDStatus.Name == "Picked");

                if (user.Role != "Admin" && user.Role != "admin" && user.Role != "0")
                {
                    if (user.Employee != null)
                    {
                        query = query.Where(x => x.LocationId == user.Employee.LocationId);
                    }
                }

                var data = query
                    .Select(x => new MEDBookingDto
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Remarks = x.Remarks,
                        Time = x.Time,
                        Date = x.Date,
                        Count = x.Count,
                        Total = x.Total,
                        Name = x.Name,
                        MobileNo = x.MobileNo,
                        EmailId = x.EmailId,
                        PickUpDate = x.PickUpDate,
                        AssignedDate = x.AssignedDate,
                        StatusDate = x.StatusDate,
                        Address = x.Address,
                        Lat = x.Lat,
                        Lng = x.Lng,
                        OrderDate = x.OrderDate,
                        IsPaid = x.IsPaid,
                        PromoOfferPrice = x.PromoOfferPrice,
                        WalletCash = x.WalletCash,
                        RefNo = x.RefNo,
                        EmployeeId = x.EmployeeId,
                        Employee = new EmployeeDto
                        {
                            Id = x.Employee != null ? x.Employee.Id : 0,
                            Name = x.Employee != null ? x.Employee.Name : "",
                        },
                        MEDStatusId = x.MEDStatusId,
                        StatusName = x.MEDStatus != null ? x.MEDStatus.Name : "",
                        LocationId = x.LocationId,
                        Location = new LocationDto
                        {
                            Id = x.Location != null ? x.Location.Id : 0,
                            Name = x.Location != null ? x.Location.Name : "",
                        },
                        SubTotal = x.SubTotal,
                        TotalDeliveryCharge = x.TotalDeliveryCharge,
                    }).OrderByDescending(x => x.Id).Skip(id).Take(10).ToList();

                return data;
            }
        }

        [OrangeAuthorizationFilter]
        [HttpGet]
        [Route("MEDAdminWebDashCounts")]
        public BookingDto MEDAdminWebDashCounts()
        {
            using (EAharaDB context = new EAharaDB())
            {
                BookingDto retdata = new BookingDto();

                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;

                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var query = context.MEDBookings.Where(x => x.IsActive &&
                                             (DbFunctions.TruncateTime(x.OrderDate)) >= (DbFunctions.TruncateTime(DateTime.Now)));

                if (user.Role != "Admin" && user.Role != "admin" && user.Role != "0")
                {
                    if (user.Employee != null)
                    {
                        query = query.Where(x => x.LocationId == user.Employee.LocationId);
                    }
                }

                retdata.DBooked = query.Where(x => x.MEDStatus.Name == "Booked").Count();

                retdata.DApproved = query.Where(x => x.MEDStatus.Name == "Approved").Count();

                retdata.DPicked = query.Where(x => x.MEDStatus.Name == "Picked").Count();

                retdata.DReady = query.Where(x => x.MEDStatus.Name == "Ready").Count();

                retdata.DDelivered = query.Where(x => x.MEDStatus.Name == "Delivered").Count();

                retdata.DCancelled = query.Where(x => x.MEDStatus.Name == "Cancelled").Count();

                return retdata;
            }
        }

        [HttpGet]
        [Route("MEDDriverDashBoardCounts/{id}")]
        public BookingDto MEDDriverDashBoardCounts(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                BookingDto retdata = new BookingDto();

                retdata.TAssigned = context.MEDBookings.Where(x => x.IsActive && x.EmployeeId == id &&
                                             (DbFunctions.TruncateTime(x.OrderDate)) >= (DbFunctions.TruncateTime(DateTime.Now))).Count();

                retdata.TDelivered = context.MEDBookings.Where(x => x.IsActive && x.EmployeeId == id &&
                                             (DbFunctions.TruncateTime(x.OrderDate)) >= (DbFunctions.TruncateTime(DateTime.Now)) && x.MEDStatus.Name != "Delivered").Count();

                return retdata;
            }
        }


        [HttpGet]
        [Route("MEDgetDriverOrdersUnDelivrerd/{id}")]
        public List<MEDBookingDto> MEDgetDriverOrdersUnDelivrerd(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDBookings.Where(x => x.IsActive == true && x.EmployeeId == id && x.MEDStatus.Name != "Delivered" && x.MEDStatus.Name != "Cancelled");

                var datalist = data
                  .Select(x => new MEDBookingDto
                  {

                      Id = x.Id,
                      Description = x.Description,
                      Remarks = x.Remarks,
                      Time = x.Time,
                      Date = x.Date,
                      Count = x.Count,
                      Total = x.Total,
                      Name = x.Name,
                      IsPaid = x.IsPaid,
                      MobileNo = x.MobileNo,
                      EmailId = x.EmailId,
                      PickUpDate = x.PickUpDate,
                      AssignedDate = x.AssignedDate,
                      StatusDate = x.StatusDate,
                      Address = x.Address,
                      PromoOfferPrice = x.PromoOfferPrice,
                      WalletCash = x.WalletCash,
                      ActualTotal = x.PromoOfferPrice + x.SubTotal,
                      Lat = x.Lat,
                      Lng = x.Lng,
                      EmployeeId = x.EmployeeId,
                      OrderDate = x.OrderDate,
                      RefNo = x.RefNo,
                      LocationId = x.LocationId,
                      Location = new LocationDto
                      {
                          Id = x.Location != null ? x.Location.Id : 0,
                          Name = x.Location != null ? x.Location.Name : "",
                      },
                      PromoOffer = new PromoOfferDto
                      {
                          Id = x.PromoOffer != null ? x.PromoOffer.Id : 0,
                          Tittle = x.PromoOffer != null ? x.PromoOffer.Tittle : "",
                      },
                      MEDStatusId = x.MEDStatusId,
                      StatusName = x.MEDStatus != null ? x.MEDStatus.Name : "",
                      SubTotal = x.SubTotal,
                      TotalDeliveryCharge = x.TotalDeliveryCharge,
                      MEDBookingDetails = x.MEDBookingDetails.Where(y => y.IsActive)
                      .Select(y => new MEDBookingDetailDto
                      {
                          Id = y.Id,
                          Quantity = y.Quantity,
                          Price = y.Price,
                          TotalPrice = y.TotalPrice,
                          DiscountPrice = y.DiscountPrice,
                          DelCharge = y.DelCharge,
                          MEDItemId = y.MEDItemId,
                          MEDShopId = y.MEDShopId,
                          Remarks = y.Remarks,
                          IsActive = true,
                          MEDItem = new MEDItemDto
                          {
                              Name = y.MEDItem != null ? y.MEDItem.Name : "",
                              Id = y.MEDItem != null ? y.MEDItem.Id : 0,
                          },
                      }).ToList(),

                  }).OrderByDescending(x => x.Id).ToList();

                return datalist;
            }
        }

        [HttpGet]
        [Route("GetMedShopSalesById/{id}/{sid}")]
        public List<MEDBookingDto> GetMedShopSalesById(long id, long sid)
        {

            using (EAharaDB context = new EAharaDB())
            {
                var shop = context.MEDShops.FirstOrDefault(x => x.Id == id);
                var data = context.MEDBookings.Where(x => x.IsActive == true && x.MEDBookingDetails.Any(y=> y.MEDShopId == id));

                if (sid > 0)
                {
                    data = data.Where(x => x.MEDStatusId == sid);
                }

                var datalist = data
                  .Select(x => new MEDBookingDto
                  {

                      Id = x.Id,
                      Description = x.Description,
                      Remarks = x.Remarks,
                      Time = x.Time,
                      Date = x.Date,
                      Count = x.Count,
                      Total = x.Total,
                      Name = x.Name,
                      IsPaid = x.IsPaid,
                      MobileNo = x.MobileNo,
                      EmailId = x.EmailId,
                      PickUpDate = x.PickUpDate,
                      AssignedDate = x.AssignedDate,
                      StatusDate = x.StatusDate,
                      Address = x.Address,
                      PromoOfferPrice = x.PromoOfferPrice,
                      WalletCash = x.WalletCash,
                      ActualTotal = x.PromoOfferPrice + x.SubTotal,
                      Lat = x.Lat,
                      Lng = x.Lng,
                      EmployeeId = x.EmployeeId,
                      OrderDate = x.OrderDate,
                      RefNo = x.RefNo,
                      LocationId = x.LocationId,
                      Location = new LocationDto
                      {
                          Id = x.Location != null ? x.Location.Id : 0,
                          Name = x.Location != null ? x.Location.Name : "",
                      },
                      PromoOffer = new PromoOfferDto
                      {
                          Id = x.PromoOffer != null ? x.PromoOffer.Id : 0,
                          Tittle = x.PromoOffer != null ? x.PromoOffer.Tittle : "",
                      },
                      MEDStatusId = x.MEDStatusId,
                      StatusName = x.MEDStatus != null ? x.MEDStatus.Name : "",
                      SubTotal = x.SubTotal,
                      TotalDeliveryCharge = x.TotalDeliveryCharge,
                      MEDBookingDetails = x.MEDBookingDetails.Where(y => y.IsActive)
                      .Select(y => new MEDBookingDetailDto
                      {
                          Id = y.Id,
                          Quantity = y.Quantity,
                          Price = y.Price,
                          TotalPrice = y.TotalPrice,
                          DiscountPrice = y.DiscountPrice,
                          DelCharge = y.DelCharge,
                          MEDItemId = y.MEDItemId,
                          MEDShopId = y.MEDShopId,
                          Remarks = y.Remarks,
                          IsActive = true,
                          MEDItem = new MEDItemDto
                          {
                              Name = y.MEDItem != null ? y.MEDItem.Name : "",
                              Id = y.MEDItem != null ? y.MEDItem.Id : 0,
                          },
                      }).ToList(),

                  }).OrderByDescending(x => x.Id).ToList();

                return datalist;
            }
        }


        [HttpGet]
        [Route("MEDSaveDelivredFromDriver/{id}")]
        public bool MEDSaveDelivredFromDriver(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDBookings.FirstOrDefault(x => x.Id == id);
                var cp = context.CompanyProfiles.FirstOrDefault(x => x.Id > 0);
                if (data != null)
                {
                    var status = context.MEDStatuses.FirstOrDefault(x => x.IsActive && x.Name == "Delivered");
                    data.MEDStatusId = status.Id;
                    context.Entry(data).Property(x => x.MEDStatusId).IsModified = true;

                    data.StatusDate = DateTime.Now;
                    context.Entry(data).Property(x => x.StatusDate).IsModified = true;

                    context.SaveChanges();

                    MessageController messagectrl = new MessageController();

                    string msg = "Your order no " + data.RefNo + " has been " + status.Name + " ," +
                        " thanks for using HF Services, Issues ? Call us on Number " + cp.MobileNo;

                    NotificationController Notictrl = new NotificationController();
                    messagectrl.sendSMS(msg, data.MobileNo);
                    if (data.CustomerId > 0)
                    {
                        Notictrl.addCustomerNotification(msg, data.CustomerId.Value);
                    }


                    return true;
                }
                return true;
            }
        }

        [HttpGet]
        [Route("MEDSavePickedFromDriver/{id}")]
        public bool MEDSavePickedFromDriver(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDBookings.FirstOrDefault(x => x.Id == id);
                var cp = context.CompanyProfiles.FirstOrDefault(x => x.Id > 0);
                if (data != null)
                {
                    var status = context.MEDStatuses.FirstOrDefault(x => x.IsActive && x.Name == "Picked");
                    data.MEDStatusId = status.Id;
                    context.Entry(data).Property(x => x.MEDStatusId).IsModified = true;

                    data.PickUpDate = DateTime.Now;
                    context.Entry(data).Property(x => x.PickUpDate).IsModified = true;

                    context.SaveChanges();

                    MessageController messagectrl = new MessageController();

                    string msg = "Your order no " + data.RefNo + " has been " + status.Name + " ," +
                        " thanks for using HF Services, Issues ? Call us on Number " + cp.MobileNo;

                    NotificationController Notictrl = new NotificationController();
                    messagectrl.sendSMS(msg, data.MobileNo);
                    if (data.CustomerId > 0)
                    {
                        Notictrl.addCustomerNotification(msg, data.CustomerId.Value);
                    }


                    return true;
                }
                return true;
            }
        }


        [HttpPost]
        [Route("MEDgetDriverOrdersUnDelivrerdByDay")]
        public List<MEDBookingDto> MEDgetDriverOrdersUnDelivrerdByDay(KendoFilterDto Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.MEDBookings.Where(x => x.IsActive == true && x.EmployeeId == Request.EmployeeId
                                            && (DbFunctions.TruncateTime(x.OrderDate)) == (DbFunctions.TruncateTime(Request.Date)));

                var datalist = data
                  .Select(x => new MEDBookingDto
                  {
                      Id = x.Id,
                      Description = x.Description,
                      Remarks = x.Remarks,
                      Time = x.Time,
                      Date = x.Date,
                      Count = x.Count,
                      Total = x.Total,
                      Name = x.Name,
                      IsPaid = x.IsPaid,
                      MobileNo = x.MobileNo,
                      PickUpDate = x.PickUpDate,
                      AssignedDate = x.AssignedDate,
                      StatusDate = x.StatusDate,
                      EmailId = x.EmailId,
                      Address = x.Address,
                      PromoOfferPrice = x.PromoOfferPrice,
                      WalletCash = x.WalletCash,
                      ActualTotal = x.PromoOfferPrice + x.SubTotal,
                      Lat = x.Lat,
                      Lng = x.Lng,
                      EmployeeId = x.EmployeeId,
                      Employee = new EmployeeDto
                      {
                          Id = x.Employee != null ? x.Employee.Id : 0,
                          Name = x.Employee != null ? x.Employee.Name : "",
                      },
                      OrderDate = x.OrderDate,
                      RefNo = x.RefNo,
                      LocationId = x.LocationId,
                      Location = new LocationDto
                      {
                          Id = x.Location != null ? x.Location.Id : 0,
                          Name = x.Location != null ? x.Location.Name : "",
                      },
                      PromoOffer = new PromoOfferDto
                      {
                          Id = x.PromoOffer != null ? x.PromoOffer.Id : 0,
                          Tittle = x.PromoOffer != null ? x.PromoOffer.Tittle : "",
                      },
                      MEDStatusId = x.MEDStatusId,
                      StatusName = x.MEDStatus != null ? x.MEDStatus.Name : "",
                      SubTotal = x.SubTotal,
                      TotalDeliveryCharge = x.TotalDeliveryCharge,
                      MEDBookingDetails = x.MEDBookingDetails.Where(y => y.IsActive)
                      .Select(y => new MEDBookingDetailDto
                      {
                          Id = y.Id,
                          Quantity = y.Quantity,
                          Price = y.Price,
                          TotalPrice = y.TotalPrice,
                          DiscountPrice = y.DiscountPrice,
                          DelCharge = y.DelCharge,
                          MEDItemId = y.MEDItemId,
                          MEDShopId = y.MEDShopId,
                          Remarks = y.Remarks,
                          IsActive = true,
                          MEDItem = new MEDItemDto
                          {
                              Name = y.MEDItem != null ? y.MEDItem.Name : "",
                              Id = y.MEDItem != null ? y.MEDItem.Id : 0,
                          },
                      }).ToList(),

                  }).OrderByDescending(x => x.Id).ToList();

                return datalist;
            }
        }



    }
}
