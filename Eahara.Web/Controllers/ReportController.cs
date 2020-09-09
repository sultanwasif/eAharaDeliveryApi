using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Eahara.Model;
using Eahara.Web.Filter;
using Eahara.Web.Dtos;
using Kendo.DynamicLinq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;


namespace Eahara.Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReportController : ApiController
    {
        [HttpPost]
        [Route("AddReport")]
        public bool AddReport(ReportDto locationDto)
        {
            if (locationDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (locationDto.Id > 0)
                    {
                        var data = context.Reports.FirstOrDefault(x => x.Id == locationDto.Id);
                        if (data != null)
                        {
                            data.RPTName = locationDto.RPTName;
                            data.Description = locationDto.Description;

                            context.Entry(data).Property(x => x.RPTName).IsModified = true;
                            context.Entry(data).Property(x => x.Description).IsModified = true;

                            context.SaveChanges();
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        Report location = new Report();

                        location.RPTName = locationDto.RPTName;
                        location.Description = locationDto.Description;

                        location.IsActive = true;
                        context.Reports.Add(location);

                        context.SaveChanges();
                        return true;
                    }

                }

            }
            return false;
        }

        [HttpPost]
        [Route("ReportsInView")]
        public DataSourceResult ReportsInView(DataSourceRequest Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var dataSourceResult = context.Reports.Where(x => x.IsActive == true)
                    .Select(x => new ReportDto
                    {
                        Id = x.Id,
                        Description = x.Description,
                        RPTName = x.RPTName,
                        IsActive = x.IsActive,

                    }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }

        [HttpGet]
        [Route("DeleteReportId/{id}")]
        public bool DeleteReportId(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.Reports.FirstOrDefault(x => x.Id == id);
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
        [Route("ReportsInDropdown")]
        public List<ReportDto> ReportsInDropdown()
        {
            List<ReportDto> DtoList;
            using (EAharaDB context = new EAharaDB())
            {
                var data = context.Reports.Where(x => x.IsActive == true)
                    .Select(x => new ReportDto
                    {
                        Id = x.Id,
                        IsActive = x.IsActive,
                        RPTName = x.RPTName,
                        Description = x.Description,

                    }).ToList();

                DtoList = data;
            }
            return DtoList;
        }

        [HttpGet]
        [Route("PrintOrder/{Id}")]
        public string PrintOrder(long Id)
        {
            ReportDocument rd = new ReportDocument();

            Guid id1 = Guid.NewGuid();
            var pdfName = "OrderReport" + id1 + ".pdf";
            string strRptPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Reports\\" + "OrderReport" + ".rpt";
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
            rd.SetParameterValue(0, Id);
            System.IO.File.Delete(strPdfPath);
            rd.PrintOptions.PaperSize = PaperSize.PaperA5;
            rd.ExportToDisk(ExportFormatType.PortableDocFormat, strPdfPath);

            return pdfName;

        }

        [OrangeAuthorizationFilter]
        [HttpPost]
        [Route("PrintBookingReportList")]
        public string PrintBookingReportList(KendoFilterRequestDto Request)
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

                long[] items = rep.Select(x => x.Id).ToArray();

                ReportDocument rd = new ReportDocument();

                Guid id1 = Guid.NewGuid();
                var pdfName = "BookingReport" + id1 + ".pdf";
                string strRptPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Reports\\" + "BookingReport" + ".rpt";
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

        [HttpPost]
        [Route("PrintClientReportList")]
        public string PrintClientReportList(KendoFilterRequestDto Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var rep = context.Customers.Where(x => x.IsActive &&
                                              ((DbFunctions.TruncateTime(x.CreatedDate)) >= (DbFunctions.TruncateTime(Request.FromDate))
                                             && (DbFunctions.TruncateTime(x.CreatedDate)) <= (DbFunctions.TruncateTime(Request.ToDate))));

                long[] items = rep.Select(x => x.Id).ToArray();

                ReportDocument rd = new ReportDocument();

                Guid id1 = Guid.NewGuid();
                var pdfName = "CustomerReport" + id1 + ".pdf";
                string strRptPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Reports\\" + "CustomerReport" + ".rpt";
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

        [HttpPost]
        [Route("PrintDyncReport")]
        public string PrintDyncReport(KendoFilterRequestDto Request)
        {
            using (EAharaDB context = new EAharaDB())
            {

                ReportDocument rd = new ReportDocument();

                DateReport dreport = new DateReport();
                dreport.From = Request.FromDate.Value;
                dreport.To = Request.ToDate.Value;
                context.DateReports.Add(dreport);
                context.SaveChanges();

                Guid id1 = Guid.NewGuid();
                var pdfName = Request.RPTName + id1 + ".pdf";
                string strRptPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Reports\\" + Request.RPTName + ".rpt";
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
                //rd.SetParameterValue(0, dreport.Id);
                rd.SetParameterValue("FromDate", Request.FromDate);
                rd.SetParameterValue("ToDate", Request.ToDate);
                System.IO.File.Delete(strPdfPath);
                //rd.PrintOptions.PaperSize = PaperSize.PaperA5;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, strPdfPath);

                return pdfName;
            }

        }

        [OrangeAuthorizationFilter]
        [HttpPost]
        [Route("PrintShopDyncReport")]
        public string PrintShopDyncReport(KendoFilterRequestDto Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                BasicAuthenticationIdentity identity = (BasicAuthenticationIdentity)User.Identity;

                var user = context.Users.FirstOrDefault(x => x.Id == identity.Id);
                var shops = context.Shops.Where(x => x.IsActive);

                if (user.Role == "Employee")
                {
                    if (user.Employee != null)
                    {
                        shops = shops.Where(x => x.LocationId == user.Employee.LocationId);
                    }
                }

                long[] items = shops.Select(x => x.Id).ToArray();

                if (Request.Shops.Count > 0)
                {
                    items = Request.Shops.ToArray();                   

                }
                //foreach (var det in Request.Shops)
                //{

                //}

                ReportDocument rd = new ReportDocument();

                DateReport dreport = new DateReport();
                dreport.From = Request.FromDate.Value;
                dreport.To = Request.ToDate.Value;
                context.DateReports.Add(dreport);
                context.SaveChanges();

               

                Guid id1 = Guid.NewGuid();
                var pdfName = Request.RPTName + id1 + ".pdf";
                string strRptPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Reports\\" + Request.RPTName + ".rpt";
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
                rd.SetParameterValue("FromDate", Request.FromDate);
                rd.SetParameterValue("ToDate", Request.ToDate);
                System.IO.File.Delete(strPdfPath);
                //rd.PrintOptions.PaperSize = PaperSize.PaperA5;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, strPdfPath);

                return pdfName;
            }

        }

        [HttpGet]
        [Route("PrintMEDOrder/{Id}")]
        public string PrintMEDOrder(long Id)
        {
            ReportDocument rd = new ReportDocument();

            Guid id1 = Guid.NewGuid();
            var pdfName = "MEDReport" + id1 + ".pdf";
            string strRptPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Reports\\" + "MEDReport" + ".rpt";
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
            rd.SetParameterValue(0, Id);
            System.IO.File.Delete(strPdfPath);
           // rd.PrintOptions.PaperSize = PaperSize.PaperA5;
            rd.ExportToDisk(ExportFormatType.PortableDocFormat, strPdfPath);

            return pdfName;

        }
        
        [HttpGet]
        [Route("PrintExpense/{Id}")]
        public string PrintExpense(long Id)
        {
            ReportDocument rd = new ReportDocument();

            Guid id1 = Guid.NewGuid();
            var pdfName = "Expense" + id1 + ".pdf";
            string strRptPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Reports\\" + "Expense" + ".rpt";
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
            rd.SetParameterValue(0, Id);
            System.IO.File.Delete(strPdfPath);
           // rd.PrintOptions.PaperSize = PaperSize.PaperA5;
            rd.ExportToDisk(ExportFormatType.PortableDocFormat, strPdfPath);

            return pdfName;

        }

        [HttpPost]
        [Route("PrintExpenseList")]
        public string PrintExpenseList(KendoFilterRequestDto Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                ReportDocument rd = new ReportDocument();

                var expenses = context.CompanyExpenses.Where(x => x.IsActive  && (DbFunctions.TruncateTime(x.Date)) >= (DbFunctions.TruncateTime(Request.FromDate)) 
                                && (DbFunctions.TruncateTime(x.Date)) <= (DbFunctions.TruncateTime(Request.ToDate)));

                long[] items = expenses.Select(x => x.Id).ToArray();

                Guid id1 = Guid.NewGuid();
                var pdfName = "ExpenseList" + id1 + ".pdf";
                string strRptPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Reports\\" + "ExpenseList" + ".rpt";
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
                //rd.PrintOptions.PaperSize = PaperSize.PaperA4;
                rd.ExportToDisk(ExportFormatType.PortableDocFormat, strPdfPath);

                return pdfName;

            }

        }

    }
}
