using Eahara.Model;
using Eahara.Web.Dtos;
using Eahara.Web.Filter;
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
    public class BookingController : ApiController
    {
        public static readonly object LockObject = new object();

        [HttpPost]
        [Route("AddBooking")]
        public BookingDto AddBooking(BookingDto dataDto)
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
                                Booking addData = new Booking();
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
                                addData.ShopId = dataDto.ShopId;
                                addData.IsActive = true;
                                addData.IsOrderLater = true;

                                var status = context.Status.FirstOrDefault(x => x.Name == "Booked");
                                var shop = context.Shops.FirstOrDefault(x => x.Id == dataDto.ShopId);

                                addData.StatusId = status.Id;

                                if (dataDto.Lng != ""  && dataDto.Lng != null && dataDto.Lat != "" && dataDto.Lat != null)
                                {
                                    var sCoord = new GeoCoordinate(Convert.ToDouble(shop.Lat), Convert.ToDouble(shop.Lng));
                                    var eCoord = new GeoCoordinate(Convert.ToDouble(dataDto.Lat), Convert.ToDouble(dataDto.Lng));

                                    var distance = sCoord.GetDistanceTo(eCoord);
                                    var distinKm = distance / 1000;
                                    if (distinKm > shop.DeliveryRange)
                                    {
                                        BookingDto retdata2 = new BookingDto();
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

                                string itemText = "";
                                addData.BookingDetails = new List<BookingDetails>();
                                foreach(var det in dataDto.BookingDetails)
                                {
                                    BookingDetails Bdetail = new BookingDetails();

                                    Bdetail.BookingId = addData.Id;
                                    Bdetail.Quantity = det.Quantity;
                                    Bdetail.Price = det.Price;
                                    Bdetail.TotalPrice = det.TotalPrice;
                                    Bdetail.ItemId = det.ItemId;
                                    Bdetail.ShopId = det.ShopId;
                                    Bdetail.DelCharge = det.DelCharge;
                                    Bdetail.DiscountPrice = det.DiscountPrice;
                                    Bdetail.IsActive = true;

                                    if (status != null)
                                    {
                                        Bdetail.StatusId = status.Id;
                                    }

                                    addData.BookingDetails.Add(Bdetail);
                                    var item = context.Items.FirstOrDefault(x => x.Id == det.ItemId);
                                    itemText = itemText + item.Name + ",";
                                }

                                MessageController messagectrl = new MessageController();
                                NotificationController Notictrl = new NotificationController();

                                string msg = "Order No " + addData.RefNo + " has been placed by customer " + addData.Name + " Thanks for using eAHARA. Issues? Call us on " + cp.MobileNo + "  or " + cp.WhatsappNo + " -EAHARA";

                                messagectrl.sendSMS(msg, shop.MobileNo);

                                if (shop.MobileNo2 != null && shop.MobileNo2 != "")
                                {
                                    messagectrl.sendSMS(msg, shop.MobileNo2);
                                }
                                if (shop.MobileNo3 != null && shop.MobileNo3 != "")
                                {
                                    messagectrl.sendSMS(msg, shop.MobileNo3);
                                }

                                Notictrl.addShopNotification(msg, shop.Id);
                                Notictrl.addAdminNotification(msg);


                                string msg2 = "Your order No : " + addData.RefNo + " has been Booked, Thanks for using Eahara. Issues ? Call us on " + cp.MobileNo ;
                                messagectrl.sendSMS(msg2, addData.MobileNo);

                                if (dataDto.CustomerId > 0)
                                {
                                    Notictrl.addCustomerNotification(msg2, dataDto.CustomerId.Value);
                                }

                                if (dataDto.CustomerId > 0 && dataDto.PromoOfferId>0)
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

                                context.Bookings.Add(addData);
                                context.SaveChanges();
                                transaction.Commit();

                                BookingDto retdata = new BookingDto();
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
        [Route("DeleteBooking/{id}")]
        public bool DeleteBookingById(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.Bookings.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {
                        Delete.IsActive = false;
                        context.Entry(Delete).Property(x => x.IsActive).IsModified = true;

                        NotificationController Notictrl = new NotificationController();
                        Notictrl.addAdminNotification("Booking Deleted : " + Delete.RefNo);
                    }
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }

        [HttpGet]
        [Route("AssignDriver/{id}/{DriverId}")]
        public bool DeleteBookingById(long id, long DriverId)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.Bookings.FirstOrDefault(x => x.Id == id);
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

        [HttpGet]
        [Route("MakePaidStatusBooking/{id}")]
        public bool MakePaidStatusBooking(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.Bookings.FirstOrDefault(x => x.Id == id);
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
                        Notictrl.addShopNotification("Booking Paid Status Changed : " + Delete.RefNo, Delete.ShopId.Value);

                    }
                    context.SaveChanges();
                    return true;
                }

            }
            return false;
        }

        [HttpGet]
        [Route("ApplyPromoOfferAdmin/{id}/{value}")]
        public bool ApplyPromoOfferAdmin(long id, float value)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.Bookings.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {
                        Delete.SubTotal = (Delete.SubTotal + Delete.PromoOfferPrice) - value;
                        Delete.PromoOfferPrice = value;                        
                        Delete.Total = ( Delete.TotalDeliveryCharge + Delete.SubTotal) - Delete.WalletCash;

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


        [HttpGet]
        [Route("ApplyDelChargeAdmin/{id}/{value}")]
        public bool ApplyDelChargeAdmin(long id, float value)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.Bookings.FirstOrDefault(x => x.Id == id);
                    if (Delete != null)
                    {
                        Delete.TotalDeliveryCharge= value;
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

        [OrangeAuthorizationFilter]
        [HttpPost]
        [Route("BookingsInView")]
        public DataSourceResult BookingsInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {

                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;

                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var query = context.Bookings.Where(x => x.IsActive == true);

                if (user.Role == "Employee")
                {
                    if (user.Employee != null)
                    {
                        query = query.Where(x => x.LocationId == user.Employee.LocationId);
                    }
                }


                var dataSourceResult = query
                    .Select(x => new BookingDto
                    {

                        Id = x.Id,
                        Description = x.Description,
                        Remarks = x.Remarks,
                        Time = x.Time,
                        Date = x.Date,
                        Count = x.Count,
                        Total = x.Total,
                        Name = x.Name,
                        IsBlackList = x.IsBlackList,
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
                        StatusId = x.StatusId,
                        PickUpDate = x.PickUpDate,
                        AssignedDate = x.AssignedDate,
                        StatusDate = x.StatusDate,
                        Status = new StatusDto
                        {
                            Id = x.Status != null ? x.Status.Id : 0,
                            Name = x.Status != null ? x.Status.Name : "",
                        },
                        EmployeeId = x.EmployeeId,
                        Employee = new EmployeeDto
                        {
                            Id = x.Employee != null ? x.Employee.Id : 0,
                            Name = x.Employee != null ? x.Employee.Name : "",
                        },
                        ShopId = x.ShopId,
                        Shop = new ShopDto
                        {
                            Id = x.Shop != null ? x.Shop.Id : 0,
                            Name = x.Shop != null ? x.Shop.Name : "",
                        },
                        SubTotal = x.SubTotal,
                        TotalDeliveryCharge = x.TotalDeliveryCharge,
                        LocationId = x.LocationId,
                        Location = new LocationDto
                        {
                            Id = x.Location != null ?  x.Location.Id : 0,
                            Name = x.Location != null ?  x.Location.Name : "",
                        },
                        CustomerId = x.CustomerId,
                        Customer = new CustomerDto
                        {
                            Id = x.Customer != null ? x.Customer.Id : 0,
                            Name = x.Customer != null ? x.Customer.Name : "",
                            BLCount = x.Customer != null ? x.Customer.BLCount : 0,
                        },
                    }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }

        [OrangeAuthorizationFilter]
        [HttpPost]
        [Route("BookingReportsInView")]
        public DataSourceResult BookingReportsInView(KendoFilterRequestDto Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;
                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var query = context.Bookings.Where(x => x.IsActive &&
                                               ((DbFunctions.TruncateTime(x.OrderDate)) >= (DbFunctions.TruncateTime(Request.FromDate))
                                              && (DbFunctions.TruncateTime(x.OrderDate)) <= (DbFunctions.TruncateTime(Request.ToDate))));
                if (user.Role == "Employee")
                {
                    if (user.Employee != null)
                    {
                        query = query.Where(x => x.LocationId == user.Employee.LocationId);
                    }
                }
                var rep = query;

                if (Request.ShopId > 0)
                {
                    rep = rep.Where(x=>x.ShopId == Request.ShopId);
                }

                if (Request.StatusId > 0)
                {
                    rep = rep.Where(x=>x.StatusId == Request.StatusId);
                }

                if (Request.LocationId > 0)
                {
                    rep = rep.Where(x=>x.LocationId == Request.LocationId);
                }

                if (Request.IsBlacklist == true)
                {
                    rep = rep.Where(x=>x.IsBlackList);
                }

                if (Request.Paid == "Paid")
                {
                    rep = rep.Where(x => x.IsPaid);
                }
                if (Request.Paid == "Not Paid")
                {
                    rep = rep.Where(x => !x.IsPaid);
                }

                var dataSourceResult = rep.Select(x => new BookingDto
                    
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
                        Lat = x.Lat,
                        Lng = x.Lng,
                        OrderDate = x.OrderDate,
                        IsPaid = x.IsPaid,
                        RefNo = x.RefNo,
                        IsBlackList = x.IsBlackList,
                        PickUpDate = x.PickUpDate,
                        AssignedDate = x.AssignedDate,
                        StatusDate = x.StatusDate,
                        PromoOfferPrice = x.PromoOfferPrice,
                        WalletCash = x.WalletCash,
                        StatusId = x.StatusId,
                        Status = new StatusDto
                        {
                            Id = x.Status != null ? x.Status.Id : 0,
                            Name = x.Status != null ? x.Status.Name : "",
                        },
                        EmployeeId = x.EmployeeId,
                        Employee = new EmployeeDto
                        {
                            Id = x.Employee != null ? x.Employee.Id : 0,
                            Name = x.Employee != null ? x.Employee.Name : "",
                        },
                        ShopId = x.ShopId,
                        Shop = new ShopDto
                        {
                            Id = x.Shop != null ? x.Shop.Id : 0,
                            Name = x.Shop != null ? x.Shop.Name : "",
                        },
                        SubTotal = x.SubTotal,
                        TotalDeliveryCharge = x.TotalDeliveryCharge,
                        LocationId = x.LocationId,
                        Location = new LocationDto
                        {
                            Id = x.Location != null ?  x.Location.Id : 0,
                            Name = x.Location != null ?  x.Location.Name : "",
                        },
                        CustomerId = x.CustomerId,
                        Customer = new CustomerDto
                        {
                            Id = x.Customer != null ? x.Customer.Id : 0,
                            Name = x.Customer != null ? x.Customer.Name : "",
                            BLCount = x.Customer != null ? x.Customer.BLCount : 0,
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
        [Route("CustomerBookingsInView")]
        public DataSourceResult CustomerBookingsInView(KendoFilterDto Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.Bookings.Where(x => x.IsActive == true && x.CustomerId == Request.CustomerId)
                    .Select(x => new BookingDto
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
                        IsBlackList = x.IsBlackList,
                        CancelRemarks = x.CancelRemarks,
                        WalletCash = x.WalletCash,
                        PromoOfferPrice = x.PromoOfferPrice,
                        Lat = x.Lat,
                        Lng = x.Lng,
                        OrderDate = x.OrderDate,
                        RefNo = x.RefNo,
                        LocationId = x.LocationId,
                        EmployeeId = x.EmployeeId,
                        Employee = new EmployeeDto
                        {
                            Id = x.Employee != null ? x.Employee.Id : 0,
                            Name = x.Employee != null ? x.Employee.Name : "",
                        },
                        StatusId = x.StatusId,
                        Status = new StatusDto
                        {
                            Id = x.Status != null ? x.Status.Id : 0,
                            Name = x.Status != null ? x.Status.Name : "",
                        },
                        Location = new LocationDto
                        {
                            Id = x.Location != null ? x.Location.Id : 0,
                            Name = x.Location != null ? x.Location.Name : "",
                        },
                        ShopId = x.ShopId,
                        Shop = new ShopDto
                        {
                            Id = x.Shop != null ? x.Shop.Id : 0,
                            Name = x.Shop != null ? x.Shop.Name : "",
                        },
                        SubTotal = x.SubTotal,
                        TotalDeliveryCharge = x.TotalDeliveryCharge,


                    }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }

        [HttpGet]
        [Route("MyOrdersInApp/{id}")]
        public List<BookingDto> MyOrdersInApp(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.Bookings.Where(x => x.IsActive == true && x.CustomerId == id)
                    .Select(x => new BookingDto
                    {

                        Id = x.Id,
                        Description = x.Description,
                        Remarks = x.Remarks,
                        Time = x.Time,
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
                        PickUpDate = x.PickUpDate,
                        AssignedDate = x.AssignedDate,
                        StatusDate = x.StatusDate,
                        Lat = x.Lat,
                        IsBlackList = x.IsBlackList,
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
                        ShopId = x.ShopId,
                        Shop = new ShopDto
                        {
                            Id = x.Shop != null ? x.Shop.Id : 0,
                            Name = x.Shop != null ? x.Shop.Name : "",
                            Image = x.Shop != null ? x.Shop.Image : "",
                        },
                        PromoOffer = new PromoOfferDto
                        {
                            Id = x.PromoOffer != null ? x.PromoOffer.Id : 0,
                            Tittle = x.PromoOffer != null ? x.PromoOffer.Tittle : "",
                        },
                        StatusId = x.StatusId,
                        StatusName = x.Status != null ? x.Status.Name : "",
                        SubTotal = x.SubTotal,
                        TotalDeliveryCharge = x.TotalDeliveryCharge,
                        BookingDetails = x.BookingDetails.Where(y=>y.IsActive)
                        .Select(y=> new BookingDetailsDto {
                            Id = y.Id,
                            Quantity = y.Quantity,
                            Price = y.Price,
                            TotalPrice = y.TotalPrice,
                            DiscountPrice = y.DiscountPrice,
                            DelCharge = y.DelCharge,
                            ItemId = y.ItemId,
                            ShopId = y.ShopId,
                            Remarks = y.Remarks,
                            StatusId = y.StatusId,
                            IsActive = true,
                            Status = new StatusDto
                            {
                                Name = y.Status != null ?  y.Status.Name : "",
                                Id = y.Status !=null ? y.Status.Id : 0,
                            },
                            Item = new ItemDto
                            {
                                Name = y.Item != null ? y.Item.Name : "",
                                Id = y.Item != null ?  y.Item.Id : 0,
                            },
                        }).ToList(),

                    }).OrderByDescending(x => x.Id).ToList();

                return dataSourceResult;
            }
        }


        [HttpGet]
        [Route("MyTracksInApp/{id}")]
        public List<BookingDto> MyTracksInApp(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.Bookings.Where(x => x.IsActive == true && x.CustomerId == id && (x.Status.Name == "Booked" || x.Status.Name == "Approved" || x.Status.Name == "Picked" || x.Status.Name == "Ready"))
                    .Select(x => new BookingDto
                    {

                        Id = x.Id,
                        Description = x.Description,
                        Remarks = x.Remarks,
                        Time = x.Time,
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
                        PickUpDate = x.PickUpDate,
                        AssignedDate = x.AssignedDate,
                        StatusDate = x.StatusDate,
                        Lat = x.Lat,
                        IsBlackList = x.IsBlackList,
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
                        ShopId = x.ShopId,
                        Shop = new ShopDto
                        {
                            Id = x.Shop != null ? x.Shop.Id : 0,
                            Name = x.Shop != null ? x.Shop.Name : "",
                            Image = x.Shop != null ? x.Shop.Image : "",
                        },
                        PromoOffer = new PromoOfferDto
                        {
                            Id = x.PromoOffer != null ? x.PromoOffer.Id : 0,
                            Tittle = x.PromoOffer != null ? x.PromoOffer.Tittle : "",
                        },
                        StatusId = x.StatusId,
                        StatusName = x.Status != null ? x.Status.Name : "",
                        SubTotal = x.SubTotal,
                        TotalDeliveryCharge = x.TotalDeliveryCharge,
                        BookingDetails = x.BookingDetails.Where(y => y.IsActive)
                        .Select(y => new BookingDetailsDto
                        {
                            Id = y.Id,
                            Quantity = y.Quantity,
                            Price = y.Price,
                            TotalPrice = y.TotalPrice,
                            DiscountPrice = y.DiscountPrice,
                            DelCharge = y.DelCharge,
                            ItemId = y.ItemId,
                            ShopId = y.ShopId,
                            Remarks = y.Remarks,
                            StatusId = y.StatusId,
                            IsActive = true,
                            Status = new StatusDto
                            {
                                Name = y.Status != null ? y.Status.Name : "",
                                Id = y.Status != null ? y.Status.Id : 0,
                            },
                            Item = new ItemDto
                            {
                                Name = y.Item != null ? y.Item.Name : "",
                                Id = y.Item != null ? y.Item.Id : 0,
                            },
                        }).ToList(),

                    }).OrderByDescending(x => x.Id).ToList();

                return dataSourceResult;
            }
        }

        [HttpPost]
        [Route("UpdateBookingStatus")]
        public bool UpdateBookingStatus(FilterDto dataDto)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Bookings.FirstOrDefault(x => x.Id == dataDto.id);
                var cp = context.CompanyProfiles.FirstOrDefault(x=>x.Id >0);
                if (data!=null)
                {
                    data.StatusId = dataDto.sid;
                    context.Entry(data).Property(x => x.StatusId).IsModified = true;
                    data.CancelRemarks = dataDto.CancelRemarks;
                    context.Entry(data).Property(x => x.CancelRemarks).IsModified = true;

                    data.StatusDate = DateTime.Now;
                    context.Entry(data).Property(x => x.StatusDate).IsModified = true;

                    context.SaveChanges();
                    
                    var status = context.Status.FirstOrDefault(x=>x.Id == dataDto.sid);
                    //if (status.Name == "Delivered")
                    //{
                    //    MessageController messagectrl = new MessageController();

                    //    string msg = "You order No "+ data.Booking.RefNo + " has been delivered, thanks for using Eahara, Issues ? Call us on Number " + cp.MobileNo + "  or " + cp.WhatsappNo; 

                    //    messagectrl.sendSMS(msg, data.Booking.MobileNo);
                    //}

                    NotificationController Notictrl = new NotificationController();
                    Notictrl.addAdminNotification("Booking Status Changed : " + data.RefNo + " to : " + status.Name);
                    Notictrl.addShopNotification("Booking Status Changed : " + data.RefNo + " to : " + status.Name, data.ShopId.Value);

                    if (status.Name == "Approved")
                    {
                        MessageController messagectrl = new MessageController();

                        string msg = "Your order " + data.RefNo 
                            + " is Approved Our executive will deliver it soon, thanks for using Eahara. Issues ? " +
                            "Call us on Number " + cp.MobileNo;

                        messagectrl.sendSMS(msg, data.MobileNo);
                        if (data.CustomerId > 0)
                        {
                            Notictrl.addCustomerNotification(msg, data.CustomerId.Value);
                        }
                    }
                    else if(status.Name == "Delivered" || status.Name == "Cancelled")
                    {
                        MessageController messagectrl = new MessageController();

                        string msg = "Your order no " + data.RefNo + " has been " + status.Name + " ," +
                            " thanks for using Eahara, Issues ? Call us on Number " + cp.MobileNo;

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
        [Route("AddOrderBlackList")]
        public bool AddOrderBlackList(FilterDto dataDto)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Bookings.FirstOrDefault(x => x.Id == dataDto.id);
                if (data != null)
                {
                    if (data.IsBlackList == true)
                    {
                        data.IsBlackList = false;
                        if (data.CustomerId > 0)
                        {
                            var customer = context.Customers.FirstOrDefault(x=> x.Id == data.CustomerId);
                            customer.BLCount = customer.BLCount - 1;
                            context.Entry(customer).Property(x => x.BLCount).IsModified = true;
                        }
                    }
                    else
                    {
                        data.IsBlackList = true;
                        if (data.CustomerId > 0)
                        {
                            var customer = context.Customers.FirstOrDefault(x => x.Id == data.CustomerId);
                            customer.BLCount = customer.BLCount + 1;
                            context.Entry(customer).Property(x => x.BLCount).IsModified = true;
                        }
                    }

                    context.Entry(data).Property(x => x.IsBlackList).IsModified = true;
                    context.SaveChanges();

                    return true;
                }
                return true;
            }
        }

        [HttpPost]
        [Route("ApplyRemarksAdmin")]
        public bool ApplyRemarksAdmin(FilterDto dataDto)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Bookings.FirstOrDefault(x => x.Id == dataDto.id);
                if (data!=null)
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
        [Route("SaveDelivredFromDriver/{id}")]
        public bool UpdateBookingStatus(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Bookings.FirstOrDefault(x => x.Id == id);
                var cp = context.CompanyProfiles.FirstOrDefault(x=>x.Id >0);
                if (data!=null)
                {
                    var status = context.Status.FirstOrDefault(x => x.IsActive && x.Name == "Delivered");
                    data.StatusId = status.Id;
                    context.Entry(data).Property(x => x.StatusId).IsModified = true;

                    data.StatusDate = DateTime.Now;
                    context.Entry(data).Property(x => x.StatusDate).IsModified = true;
                    
                    context.SaveChanges();

                    MessageController messagectrl = new MessageController();

                    string msg = "Your order no " + data.RefNo + " has been " + status.Name + " ," +
                        " thanks for using Eahara, Issues ? Call us on Number " + cp.MobileNo;

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
        [Route("SavePickedFromDriver/{id}")]
        public bool SavePickedFromDriver(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Bookings.FirstOrDefault(x => x.Id == id);
                var cp = context.CompanyProfiles.FirstOrDefault(x => x.Id > 0);
                if (data != null)
                {
                    var status = context.Status.FirstOrDefault(x => x.IsActive && x.Name == "Picked");
                    data.StatusId = status.Id;
                    context.Entry(data).Property(x => x.StatusId).IsModified = true;

                    data.PickUpDate = DateTime.Now;
                    context.Entry(data).Property(x => x.PickUpDate).IsModified = true;
                    
                    context.SaveChanges();

                    MessageController messagectrl = new MessageController();

                    string msg = "Your order no " + data.RefNo + " has been " + status.Name + " ," +
                        " thanks for using Eahara, Issues ? Call us on Number " + cp.MobileNo;

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
        [Route("CancelBooking")]
        public bool CancelBooking(FilterDto dataDto)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Bookings.FirstOrDefault(x => x.Id == dataDto.id);
                var status = context.Status.FirstOrDefault(x => x.Name == "Cancelled");
                if (data != null)
                {
                    if (status != null)
                    {
                        data.StatusId = status.Id;
                        context.Entry(data).Property(x => x.StatusId).IsModified = true;
                        data.CancelRemarks = dataDto.CancelRemarks;
                        context.Entry(data).Property(x => x.Remarks).IsModified = true;

                        data.StatusDate = DateTime.Now;
                        context.Entry(data).Property(x => x.StatusDate).IsModified = true;

                        context.SaveChanges();

                        NotificationController Notictrl = new NotificationController();
                        Notictrl.addAdminNotification("Booking Cancelled : " + data.RefNo);
                        Notictrl.addShopNotification("Booking Cancelled : " + data.RefNo , data.ShopId.Value);
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
        [Route("DashBoardCounts/{id}")]
        public FilterDto DashBoardCounts(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                FilterDto filterdto = new FilterDto();

                filterdto.Cancelled = context.Bookings.Where(x => (DbFunctions.TruncateTime(x.OrderDate)) == (DbFunctions.TruncateTime(DateTime.Now)) &&  x.ShopId == id && x.Status.Name == "Cancelled").Count();
                filterdto.Delivered = context.Bookings.Where(x => (DbFunctions.TruncateTime(x.OrderDate)) == (DbFunctions.TruncateTime(DateTime.Now)) && x.ShopId == id && x.Status.Name == "Delivered").Count();
                filterdto.Approved = context.Bookings.Where(x => (DbFunctions.TruncateTime(x.OrderDate)) == (DbFunctions.TruncateTime(DateTime.Now)) && x.ShopId == id && x.Status.Name == "Approved").Count();
                filterdto.Packed = context.Bookings.Where(x => (DbFunctions.TruncateTime(x.OrderDate)) == (DbFunctions.TruncateTime(DateTime.Now)) && x.ShopId == id && x.Status.Name == "Packed").Count();
                filterdto.New = context.Bookings.Where(x => (DbFunctions.TruncateTime(x.OrderDate)) == (DbFunctions.TruncateTime(DateTime.Now)) &&  x.ShopId == id && x.Status.Name == "Booked").Count();
             
                return filterdto;
            }
        }

        [HttpGet]
        [Route("TrackOrder/{id}")]
        public BookingDto TrackOrder(string id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Bookings.Where(x => x.IsActive == true && x.RefNo == id)
                  .Select(x => new BookingDto
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
                      PromoOfferPrice = x.PromoOfferPrice,
                      Lat = x.Lat,
                      Lng = x.Lng,
                      IsPaid = x.IsPaid,
                      OrderDate = x.OrderDate,
                      IsBlackList = x.IsBlackList,
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
                      ShopId = x.ShopId,
                      Shop = new ShopDto
                      {
                          Id = x.Shop != null ? x.Shop.Id : 0,
                          Name = x.Shop != null ? x.Shop.Name : "",
                      },
                      StatusId = x.StatusId,
                      StatusName = x.Status != null ? x.Status.Name : "",
                      SubTotal = x.SubTotal,
                      TotalDeliveryCharge = x.TotalDeliveryCharge,
                      BookingDetails = x.BookingDetails.Where(y => y.IsActive)
                      .Select(y => new BookingDetailsDto
                      {
                          Id = y.Id,
                          Quantity = y.Quantity,
                          Price = y.Price,
                          TotalPrice = y.TotalPrice,
                          DiscountPrice = y.DiscountPrice,
                          DelCharge = y.DelCharge,
                          ItemId = y.ItemId,
                          ShopId = y.ShopId,
                          Remarks = y.Remarks,
                          StatusId = y.StatusId,
                          IsActive = true,
                          Status = new StatusDto
                          {
                              Name = y.Status != null ? y.Status.Name : "",
                              Id = y.Status != null ? y.Status.Id : 0,
                          },
                          Item = new ItemDto
                          {
                              Name = y.Item != null ? y.Item.Name : "",
                              Id = y.Item != null ? y.Item.Id : 0,
                          },
                      }).ToList(),

                  }).FirstOrDefault();


                return data;
            }
        }

        [HttpGet]
        [Route("BookingsInDashboard/{id}")]
        public List<BookingDto> BookingsInDashboard(int id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Bookings.Where(x => x.IsActive)
                    .Select(x => new BookingDto
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
                        IsBlackList = x.IsBlackList,
                        Address = x.Address,
                        Lat = x.Lat,
                        Lng = x.Lng,
                        OrderDate = x.OrderDate,
                        IsPaid = x.IsPaid,
                        PromoOfferPrice = x.PromoOfferPrice,
                        WalletCash = x.WalletCash,
                        RefNo = x.RefNo,
                        StatusId = x.StatusId,
                        StatusName = x.Status != null ? x.Status.Name : "",
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
                        ShopId = x.ShopId,
                        Shop = new ShopDto
                        {
                            Id = x.Shop != null ? x.Shop.Id : 0,
                            Name = x.Shop != null ? x.Shop.Name : "",
                        },
                        CustomerId = x.CustomerId,
                        Customer = new CustomerDto
                        {
                            Id = x.Customer != null ? x.Customer.Id : 0,
                            Name = x.Customer != null ? x.Customer.Name : "",
                            BLCount = x.Customer != null ? x.Customer.BLCount : 0,
                        },
                        SubTotal = x.SubTotal,
                        TotalDeliveryCharge = x.TotalDeliveryCharge,
                    }).OrderByDescending(x => x.Id).Skip(id).Take(3).ToList();

                return data;
            }
        }

        [OrangeAuthorizationFilter]
        [HttpGet]
        [Route("BookedBookingsInDashboard/{id}")]
        public List<BookingDto> BookedBookingsInDashboard(int id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;

                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var query = context.Bookings.Where(x => x.IsActive && x.Status.Name == "Booked");

                if (user.Role != "Admin" && user.Role != "admin" && user.Role != "0")
                {
                    if (user.Employee != null)
                    {
                        query = query.Where(x => x.LocationId == user.Employee.LocationId);
                    }
                }
          

                var data = query
                    .Select(x => new BookingDto
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Remarks = x.Remarks,
                        Time = x.Time,
                        Date = x.Date,
                        Count = x.Count,
                        PickUpDate = x.PickUpDate,
                        IsBlackList = x.IsBlackList,
                        AssignedDate = x.AssignedDate,
                        StatusDate = x.StatusDate,
                        Total = x.Total,
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
                        StatusId = x.StatusId,
                        StatusName = x.Status != null ? x.Status.Name : "",
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
                        ShopId = x.ShopId,
                        Shop = new ShopDto
                        {
                            Id = x.Shop != null ? x.Shop.Id : 0,
                            Name = x.Shop != null ? x.Shop.Name : "",
                        },
                        CustomerId = x.CustomerId,
                        Customer = new CustomerDto
                        {
                            Id = x.Customer != null ? x.Customer.Id : 0,
                            Name = x.Customer != null ? x.Customer.Name : "",
                            BLCount = x.Customer != null ? x.Customer.BLCount : 0,
                        },
                        SubTotal = x.SubTotal,
                        TotalDeliveryCharge = x.TotalDeliveryCharge,
                    }).OrderByDescending(x => x.Id).Skip(id).Take(10).ToList();

                return data;
            }
        }
        
        [HttpGet]
        [Route("GetBlockListedItemsByCustomer/{id}")]
        public List<BookingDto> GetBlockListedItemsByCustomer(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {

                var query = context.Bookings.Where(x => x.IsActive && x.IsBlackList && x.CustomerId == id);
       
                var data = query
                    .Select(x => new BookingDto
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
                        IsBlackList = x.IsBlackList,
                        Lat = x.Lat,
                        Lng = x.Lng,
                        OrderDate = x.OrderDate,
                        IsPaid = x.IsPaid,
                        PromoOfferPrice = x.PromoOfferPrice,
                        WalletCash = x.WalletCash,
                        RefNo = x.RefNo,
                        StatusId = x.StatusId,
                        StatusName = x.Status != null ? x.Status.Name : "",
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
                        ShopId = x.ShopId,
                        Shop = new ShopDto
                        {
                            Id = x.Shop != null ? x.Shop.Id : 0,
                            Name = x.Shop != null ? x.Shop.Name : "",
                        },
                        CustomerId = x.CustomerId,
                        Customer = new CustomerDto
                        {
                            Id = x.Customer != null ? x.Customer.Id : 0,
                            Name = x.Customer != null ? x.Customer.Name : "",
                            BLCount = x.Customer != null ? x.Customer.BLCount : 0,
                        },
                        SubTotal = x.SubTotal,
                        TotalDeliveryCharge = x.TotalDeliveryCharge,
                    }).OrderByDescending(x => x.Id).ToList();

                return data;
            }
        }

        [OrangeAuthorizationFilter]
        [HttpGet]
        [Route("ApprovedBookingsInDashboard/{id}")]
        public List<BookingDto> ApprovedBookingsInDashboard(int id)
        {
            using (EAharaDB context = new EAharaDB())
            {

                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;

                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var query = context.Bookings.Where(x => x.IsActive && x.Status.Name == "Approved");

                if (user.Role != "Admin" && user.Role != "admin" && user.Role != "0")
                {
                    if (user.Employee != null)
                    {
                        query = query.Where(x => x.LocationId == user.Employee.LocationId);
                    }
                }


                var data = query
                    .Select(x => new BookingDto
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
                        IsBlackList = x.IsBlackList,
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
                        StatusId = x.StatusId,
                        StatusName = x.Status != null ? x.Status.Name : "",
                        LocationId = x.LocationId,
                        Location = new LocationDto
                        {
                            Id = x.Location != null ? x.Location.Id : 0,
                            Name = x.Location != null ? x.Location.Name : "",
                        },
                        ShopId = x.ShopId,
                        Shop = new ShopDto
                        {
                            Id = x.Shop != null ? x.Shop.Id : 0,
                            Name = x.Shop != null ? x.Shop.Name : "",
                        },
                        SubTotal = x.SubTotal,
                        CustomerId = x.CustomerId,
                        Customer = new CustomerDto
                        {
                            Id = x.Customer != null ? x.Customer.Id : 0,
                            Name = x.Customer != null ? x.Customer.Name : "",
                            BLCount = x.Customer != null ? x.Customer.BLCount : 0,
                        },
                        TotalDeliveryCharge = x.TotalDeliveryCharge,
                    }).OrderByDescending(x => x.Id).Skip(id).Take(10).ToList();

                return data;
            }
        }

        [OrangeAuthorizationFilter]
        [HttpGet]
        [Route("ReadyBookingsInDashboard/{id}")]
        public List<BookingDto> ReadyBookingsInDashboard(int id)
        {
            using (EAharaDB context = new EAharaDB())
            {

                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;

                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var query = context.Bookings.Where(x => x.IsActive && x.Status.Name == "Ready");

                if (user.Role != "Admin" && user.Role != "admin" && user.Role != "0")
                {
                    if (user.Employee != null)
                    {
                        query = query.Where(x => x.LocationId == user.Employee.LocationId);
                    }
                }


                var data = query
                    .Select(x => new BookingDto
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
                        PickUpDate = x.PickUpDate,
                        AssignedDate = x.AssignedDate,
                        StatusDate = x.StatusDate,
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
                        IsBlackList = x.IsBlackList,
                        PromoOfferPrice = x.PromoOfferPrice,
                        WalletCash = x.WalletCash,
                        RefNo = x.RefNo,
                        StatusId = x.StatusId,
                        StatusName = x.Status != null ? x.Status.Name : "",
                        LocationId = x.LocationId,
                        Location = new LocationDto
                        {
                            Id = x.Location != null ? x.Location.Id : 0,
                            Name = x.Location != null ? x.Location.Name : "",
                        },
                        ShopId = x.ShopId,
                        Shop = new ShopDto
                        {
                            Id = x.Shop != null ? x.Shop.Id : 0,
                            Name = x.Shop != null ? x.Shop.Name : "",
                        },
                        CustomerId = x.CustomerId,
                        Customer = new CustomerDto
                        {
                            Id = x.Customer != null ? x.Customer.Id : 0,
                            Name = x.Customer != null ? x.Customer.Name : "",
                            BLCount = x.Customer != null ? x.Customer.BLCount : 0,
                        },
                        SubTotal = x.SubTotal,
                        TotalDeliveryCharge = x.TotalDeliveryCharge,
                    }).OrderByDescending(x => x.Id).Skip(id).Take(10).ToList();

                return data;
            }
        }

        [OrangeAuthorizationFilter]
        [HttpGet]
        [Route("PickedBookingsInDashboard/{id}")]
        public List<BookingDto> PickedBookingsInDashboard(int id)
        {
            using (EAharaDB context = new EAharaDB())
            {

                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;

                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var query = context.Bookings.Where(x => x.IsActive && x.Status.Name == "Picked");

                if (user.Role != "Admin" && user.Role != "admin" && user.Role != "0")
                {
                    if (user.Employee != null)
                    {
                        query = query.Where(x => x.LocationId == user.Employee.LocationId);
                    }
                }


                var data = query
                    .Select(x => new BookingDto
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Remarks = x.Remarks,
                        Time = x.Time,
                        Date = x.Date,
                        IsBlackList = x.IsBlackList,
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
                        EmployeeId = x.EmployeeId,
                        Employee = new EmployeeDto
                        {
                            Id = x.Employee != null ? x.Employee.Id : 0,
                            Name = x.Employee != null ? x.Employee.Name : "",
                        },
                        StatusId = x.StatusId,
                        StatusName = x.Status != null ? x.Status.Name : "",
                        LocationId = x.LocationId,
                        Location = new LocationDto
                        {
                            Id = x.Location != null ? x.Location.Id : 0,
                            Name = x.Location != null ? x.Location.Name : "",
                        },
                        ShopId = x.ShopId,
                        Shop = new ShopDto
                        {
                            Id = x.Shop != null ? x.Shop.Id : 0,
                            Name = x.Shop != null ? x.Shop.Name : "",
                        },
                        CustomerId = x.CustomerId,
                        Customer = new CustomerDto
                        {
                            Id = x.Customer != null ? x.Customer.Id : 0,
                            Name = x.Customer != null ? x.Customer.Name : "",
                            BLCount = x.Customer != null ? x.Customer.BLCount : 0,
                        },
                        SubTotal = x.SubTotal,
                        TotalDeliveryCharge = x.TotalDeliveryCharge,
                    }).OrderByDescending(x => x.Id).Skip(id).Take(10).ToList();

                return data;
            }
        }

        [OrangeAuthorizationFilter]
        [HttpGet]
        [Route("AdminWebDashCounts")]
        public BookingDto AdminWebDashCounts()
        {
            using (EAharaDB context = new EAharaDB())
            {

                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;

                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var query = context.Bookings.Where(x => x.IsActive &&
                                             (DbFunctions.TruncateTime(x.OrderDate)) >= (DbFunctions.TruncateTime(DateTime.Now)));

                if (user.Role != "Admin" && user.Role != "admin" && user.Role != "0")
                {
                    if (user.Employee != null)
                    {
                        query = query.Where(x => x.LocationId == user.Employee.LocationId);
                    }
                }

                BookingDto retdata = new BookingDto();

                retdata.DBooked = query.Where(x => x.Status.Name == "Booked").Count();

                retdata.DApproved = query.Where(x => x.Status.Name == "Approved").Count();

                retdata.DPicked = query.Where(x =>  x.Status.Name == "Picked").Count();

                retdata.DReady = query.Where(x =>  x.Status.Name == "Ready").Count();

                retdata.DDelivered = query.Where(x => x.Status.Name == "Delivered").Count();

                retdata.DCancelled = query.Where(x => x.Status.Name == "Cancelled").Count();

                return retdata;
            }
        }

        [HttpGet]
        [Route("DriverDashBoardCounts/{id}")]
        public BookingDto DriverDashBoardCounts(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                BookingDto retdata = new BookingDto();

                retdata.TAssigned = context.Bookings.Where(x => x.IsActive && x.EmployeeId == id &&
                                             (DbFunctions.TruncateTime(x.OrderDate)) >= (DbFunctions.TruncateTime(DateTime.Now))).Count();

                retdata.TDelivered = context.Bookings.Where(x => x.IsActive && x.EmployeeId == id &&
                                             (DbFunctions.TruncateTime(x.OrderDate)) >= (DbFunctions.TruncateTime(DateTime.Now)) && x.Status.Name != "Delivered").Count();

                return retdata;
            }
        }

        [HttpGet]
        [Route("BookingDetailsById/{id}")]
        public BookingDto BookingDetailsById(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Bookings.FirstOrDefault(x => x.IsActive == true && x.Id == id);

                BookingDto bkdto = new BookingDto();

                bkdto.Id = data.Id;
                bkdto.Description = data.Description;
                bkdto.Remarks = data.Remarks;
                bkdto.Time = data.Time;
                bkdto.Date = data.Date;
                bkdto.Count = data.Count;
                bkdto.Total = data.Total;
                bkdto.Name = data.Name;
                bkdto.IsPaid = data.IsPaid;
                bkdto.IsBlackList = data.IsBlackList;
                bkdto.MobileNo = data.MobileNo;
                bkdto.PickUpDate = data.PickUpDate;
                bkdto.AssignedDate = data.AssignedDate;
                bkdto.StatusDate = data.StatusDate;
                bkdto.EmailId = data.EmailId;
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
                bkdto.StatusId = data.StatusId;
                bkdto.Status = new StatusDto
                {
                    Id = data.Status != null ? data.Status.Id : 0,
                    Name = data.Status != null ? data.Status.Name : "",
                };
                bkdto.ShopId = data.ShopId;
                bkdto.Shop = new ShopDto
                {
                    Id = data.Shop != null ? data.Shop.Id : 0,
                    Name = data.Shop != null ? data.Shop.Name : "",
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

                bkdto.BookingDetails = context.BokkingDetailes.Where(y => y.IsActive == true && y.BookingId == data.Id)
                                .Select(y => new BookingDetailsDto
                                {
                                    Id = y.Id,
                                    Quantity = y.Quantity,
                                    Price = y.Price,
                                    TotalPrice = y.TotalPrice,
                                    DiscountPrice = y.DiscountPrice,
                                    DelCharge = y.DelCharge,
                                    Remarks = y.Remarks,
                                    ItemId = y.ItemId,
                                    ShopId = y.ShopId,
                                    StatusId = y.StatusId,
                                    IsActive = true,
                                    Status = new StatusDto
                                    {
                                        Name = y.Status != null ? y.Status.Name : "",
                                        Id = y.Status != null ? y.Status.Id : 0,
                                    },
                                    Shop = new ShopDto
                                    {
                                        Name = y.Shop.Name,
                                        Id = y.Shop.Id,
                                    },

                                    Item = new ItemDto
                                    {
                                        Name = y.Item.Name,
                                        Id = y.Item.Id,
                                    },

                                }).ToList();
                
                return bkdto;
            }
        }

        [HttpGet]
        [Route("GetShopSalesById/{id}/{sid}")]
        public List<BookingDto> GetShopSalesById(long id, long sid)
        {
          
            using (EAharaDB context = new EAharaDB())
            {
                var shop = context.Shops.FirstOrDefault(x=>x.Id == id);
                var data = context.Bookings.Where(x => x.IsActive == true && x.ShopId == id);

                if (sid > 0)
                {
                    data = data.Where(x=>x.StatusId == sid);
                }

                var datalist = data
                  .Select(x => new BookingDto
                  {

                      Id = x.Id,
                      Description = x.Description,
                      Remarks = x.Remarks,
                      Time = x.Time,
                      Date = x.Date,
                      Count = x.Count,
                      Total = x.Total,
                      Name = x.Name,
                      IsBlackList = x.IsBlackList,
                      IsPaid = x.IsPaid,
                      MobileNo = x.MobileNo,
                      EmailId = x.EmailId,
                      Address = x.Address,
                      PickUpDate = x.PickUpDate,
                      AssignedDate = x.AssignedDate,
                      StatusDate = x.StatusDate,
                      PromoOfferPrice = x.PromoOfferPrice,
                      WalletCash = x.WalletCash,
                      ActualTotal = x.PromoOfferPrice + x.SubTotal,
                      Commission = ((x.PromoOfferPrice + x.SubTotal) * shop.CommissionPercentage) / 100,
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
                      ShopId = x.ShopId,
                      Shop = new ShopDto
                      {
                          Id = x.Shop != null ? x.Shop.Id : 0,
                          Name = x.Shop != null ? x.Shop.Name : "",
                      },
                      StatusId = x.StatusId,
                      StatusName = x.Status != null ? x.Status.Name : "",
                      SubTotal = x.SubTotal,
                      TotalDeliveryCharge = x.TotalDeliveryCharge,
                      BookingDetails = x.BookingDetails.Where(y => y.IsActive)
                      .Select(y => new BookingDetailsDto
                      {
                          Id = y.Id,
                          Quantity = y.Quantity,
                          Price = y.Price,
                          TotalPrice = y.TotalPrice,
                          DiscountPrice = y.DiscountPrice,
                          DelCharge = y.DelCharge,
                          ItemId = y.ItemId,
                          ShopId = y.ShopId,
                          Remarks = y.Remarks,
                          StatusId = y.StatusId,
                          IsActive = true,
                          Status = new StatusDto
                          {
                              Name = y.Status != null ? y.Status.Name : "",
                              Id = y.Status != null ? y.Status.Id : 0,
                          },
                          Item = new ItemDto
                          {
                              Name = y.Item != null ? y.Item.Name : "",
                              Id = y.Item != null ? y.Item.Id : 0,
                          },
                      }).ToList(),

                  }).OrderByDescending(x=>x.Id).ToList();
                
                return datalist;
            }
        }

        [HttpGet]
        [Route("getDriverOrdersUnDelivrerd/{id}")]
        public List<BookingDto> getDriverOrdersUnDelivrerd(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Bookings.Where(x => x.IsActive == true && x.EmployeeId == id && x.Status.Name != "Delivered" && x.Status.Name != "Cancelled");

                var datalist = data
                  .Select(x => new BookingDto
                  {

                      Id = x.Id,
                      Description = x.Description,
                      Remarks = x.Remarks,
                      Time = x.Time,
                      IsBlackList = x.IsBlackList,
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
                      ShopId = x.ShopId,
                      Shop = new ShopDto
                      {
                          Id = x.Shop != null ? x.Shop.Id : 0,
                          Name = x.Shop != null ? x.Shop.Name : "",
                      },
                      StatusId = x.StatusId,
                      StatusName = x.Status != null ? x.Status.Name : "",
                      SubTotal = x.SubTotal,
                      TotalDeliveryCharge = x.TotalDeliveryCharge,
                      BookingDetails = x.BookingDetails.Where(y => y.IsActive)
                      .Select(y => new BookingDetailsDto
                      {
                          Id = y.Id,
                          Quantity = y.Quantity,
                          Price = y.Price,
                          TotalPrice = y.TotalPrice,
                          DiscountPrice = y.DiscountPrice,
                          DelCharge = y.DelCharge,
                          ItemId = y.ItemId,
                          ShopId = y.ShopId,
                          Remarks = y.Remarks,
                          StatusId = y.StatusId,
                          IsActive = true,
                          Status = new StatusDto
                          {
                              Name = y.Status != null ? y.Status.Name : "",
                              Id = y.Status != null ? y.Status.Id : 0,
                          },
                          Item = new ItemDto
                          {
                              Name = y.Item != null ? y.Item.Name : "",
                              Id = y.Item != null ? y.Item.Id : 0,
                          },
                      }).ToList(),

                  }).OrderByDescending(x => x.Id).ToList();

                return datalist;
            }
        }


        [HttpPost]
        [Route("getDriverOrdersUnDelivrerdByDay")]
        public List<BookingDto> getDriverOrdersUnDelivrerdByDay(KendoFilterDto Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Bookings.Where(x => x.IsActive == true && x.EmployeeId == Request.EmployeeId 
                                            && (DbFunctions.TruncateTime(x.OrderDate)) == (DbFunctions.TruncateTime(Request.Date)) );

                var datalist = data
                  .Select(x => new BookingDto
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
                      IsPaid = x.IsPaid,
                      MobileNo = x.MobileNo,
                      EmailId = x.EmailId,
                      IsBlackList = x.IsBlackList,
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
                      ShopId = x.ShopId,
                      Shop = new ShopDto
                      {
                          Id = x.Shop != null ? x.Shop.Id : 0,
                          Name = x.Shop != null ? x.Shop.Name : "",
                      },
                      StatusId = x.StatusId,
                      StatusName = x.Status != null ? x.Status.Name : "",
                      SubTotal = x.SubTotal,
                      TotalDeliveryCharge = x.TotalDeliveryCharge,
                      BookingDetails = x.BookingDetails.Where(y => y.IsActive)
                      .Select(y => new BookingDetailsDto
                      {
                          Id = y.Id,
                          Quantity = y.Quantity,
                          Price = y.Price,
                          TotalPrice = y.TotalPrice,
                          DiscountPrice = y.DiscountPrice,
                          DelCharge = y.DelCharge,
                          ItemId = y.ItemId,
                          ShopId = y.ShopId,
                          Remarks = y.Remarks,
                          StatusId = y.StatusId,
                          IsActive = true,
                          Status = new StatusDto
                          {
                              Name = y.Status != null ? y.Status.Name : "",
                              Id = y.Status != null ? y.Status.Id : 0,
                          },
                          Item = new ItemDto
                          {
                              Name = y.Item != null ? y.Item.Name : "",
                              Id = y.Item != null ? y.Item.Id : 0,
                          },
                      }).ToList(),

                  }).OrderByDescending(x => x.Id).ToList();

                return datalist;
            }
        }


        //[HttpPost]
        //[Route("GetShopSalesInView")]
        //public DataSourceResult GetShopSalesInView(KendoFilterDto Request)
        //{
        //    using (EAharaDB context = new EAharaDB())
        //    {
        //        var dataSourceResult = context.BokkingDetailes.Where(x => x.IsActive == true && x.Booking.IsActive && x.ShopId == Request.ShopId)
        //            .Select(x => new BookingDetailsDto
        //            {
        //                Id = x.Id,
        //                Quantity = x.Quantity,
        //                Price = x.Price,
        //                TotalPrice = x.TotalPrice,
        //                DiscountPrice = x.DiscountPrice,
        //                Remarks = x.Remarks,
        //                DelCharge = x.DelCharge,
        //                BookingId = x.BookingId,
        //                StatusId = x.StatusId,
        //                Booking = new BookingDto
        //                {
        //                    Id = x.Booking.Id,
        //                    Date = x.Booking.Date,
        //                    RefNo = x.Booking.RefNo,
        //                    Name = x.Booking.Name,
        //                    MobileNo = x.Booking.MobileNo,
        //                    StatusId = x.StatusId,
        //                    StatusName = x.Status != null ? x.Status.Name : "",
        //                    PromoOfferPrice = x.Booking.PromoOfferPrice,
        //                    SubTotal = x.Booking.SubTotal,
        //                    TotalDeliveryCharge = x.Booking.TotalDeliveryCharge,
        //                },
        //                ItemId = x.ItemId,
        //                Item = new ItemDto
        //                {
        //                    Id = x.Item.Id,
        //                    Name = x.Item.Name,
        //                },
        //                Status = new StatusDto
        //                {
        //                    Name = x.Status != null ? x.Status.Name : "",
        //                    Id = x.Status != null ? x.Status.Id : 0,
        //                },
        //                IsActive = x.IsActive,
        //            }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

        //        DataSourceResult kendoResponseDto = new DataSourceResult();
        //        kendoResponseDto.Data = dataSourceResult.Data;
        //        kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
        //        kendoResponseDto.Total = dataSourceResult.Total;
        //        return kendoResponseDto;
        //    }
        //}

        [HttpPost]
        [Route("GetShopSalesInView")]
        public DataSourceResult GetShopSalesInView(KendoFilterDto Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.Bookings.Where(x => x.IsActive == true  && x.ShopId == Request.ShopId)
                    .Select(x => new BookingDto
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
                        IsBlackList = x.IsBlackList,
                        Address = x.Address,
                        Lat = x.Lat,
                        Lng = x.Lng,
                        IsPaid = x.IsPaid,
                        PickUpDate = x.PickUpDate,
                        AssignedDate = x.AssignedDate,
                        StatusDate = x.StatusDate,
                        OrderDate = x.OrderDate,
                        RefNo = x.RefNo,
                        PromoOfferPrice = x.PromoOfferPrice,
                        WalletCash = x.WalletCash,
                        StatusId = x.StatusId,
                        Status = new StatusDto
                        {
                            Id = x.Status != null ? x.Status.Id : 0,
                            Name = x.Status != null ? x.Status.Name : "",
                        },
                        ShopId = x.ShopId,
                        EmployeeId = x.EmployeeId,
                        Employee = new EmployeeDto
                        {
                            Id = x.Employee != null ? x.Employee.Id : 0,
                            Name = x.Employee != null ? x.Employee.Name : "",
                        },
                        Shop = new ShopDto
                        {
                            Id = x.Shop != null ? x.Shop.Id : 0,
                            Name = x.Shop != null ? x.Shop.Name : "",
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
        [Route("BookingReportsShop")]
        public List<BookingDto> BookingReportsShop(KendoFilterRequestDto Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var shop = context.Shops.FirstOrDefault(x => x.Id == Request.ShopId);
                var rep = context.Bookings.Where(x => x.IsActive &&
                                               ((DbFunctions.TruncateTime(x.OrderDate)) >= (DbFunctions.TruncateTime(Request.FromDate))
                                              && (DbFunctions.TruncateTime(x.OrderDate)) <= (DbFunctions.TruncateTime(Request.ToDate))));

                if (Request.ShopId > 0)
                {
                    rep = rep.Where(x => x.ShopId == Request.ShopId);
                }

                if (Request.Paid == "Paid")
                {
                    rep = rep.Where(x => x.IsPaid);
                }
                if (Request.Paid == "Not Paid")
                {
                    rep = rep.Where(x => !x.IsPaid);
                }

                var dataSourceResult = rep.Select(x => new BookingDto
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
                    Address = x.Address,
                    Lat = x.Lat,
                    Lng = x.Lng,
                    IsBlackList = x.IsBlackList,
                    OrderDate = x.OrderDate,
                    IsPaid = x.IsPaid,
                    RefNo = x.RefNo,
                    PromoOfferPrice = x.PromoOfferPrice,
                    WalletCash = x.WalletCash,
                    ActualTotal = x.PromoOfferPrice + x.SubTotal,
                    Commission = ((x.PromoOfferPrice + x.SubTotal) * shop.CommissionPercentage) / 100,
                    StatusId = x.StatusId,
                    EmployeeId = x.EmployeeId,
                    Employee = new EmployeeDto
                    {
                        Id = x.Employee != null ? x.Employee.Id : 0,
                        Name = x.Employee != null ? x.Employee.Name : "",
                    },
                    Status = new StatusDto
                    {
                        Id = x.Status != null ? x.Status.Id : 0,
                        Name = x.Status != null ? x.Status.Name : "",
                    },
                    ShopId = x.ShopId,
                    SubTotal = x.SubTotal,
                    TotalDeliveryCharge = x.TotalDeliveryCharge,
                    LocationId = x.LocationId,
                    Location = new LocationDto
                    {
                        Id = x.Location != null ? x.Location.Id : 0,
                        Name = x.Location != null ? x.Location.Name : "",
                    },
                    BookingDetails = x.BookingDetails.Where(y => y.IsActive)
                      .Select(y => new BookingDetailsDto
                      {
                          Id = y.Id,
                          Quantity = y.Quantity,
                          Price = y.Price,
                          TotalPrice = y.TotalPrice,
                          DiscountPrice = y.DiscountPrice,
                          DelCharge = y.DelCharge,
                          ItemId = y.ItemId,
                          ShopId = y.ShopId,
                          Remarks = y.Remarks,
                          StatusId = y.StatusId,
                          IsActive = true,
                          Item = new ItemDto
                          {
                              Name = y.Item != null ? y.Item.Name : "",
                              Id = y.Item != null ? y.Item.Id : 0,
                          },
                      }).ToList(),
                }).OrderByDescending(x => x.Id).ToList();

               
                return dataSourceResult;
            }
        }


    }
}
