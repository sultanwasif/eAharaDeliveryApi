using Eahara.Model;
using Eahara.Web.Dtos;
using Kendo.DynamicLinq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Eahara.Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CompanyExpenseController : ApiController
    {

        [HttpPost]
        [Route("AddCompanyExpense")]
        public bool AddCompanyExpense(CompanyExpenseDto itemDto)
        {
            if (itemDto != null)
            {
                using (EAharaDB context = new EAharaDB())
                {
                    if (itemDto.Id > 0)
                    {
                        var data = context.CompanyExpenses.FirstOrDefault(x => x.Id == itemDto.Id);
                        if (data != null)
                        {
                            data.Date = itemDto.Date;
                            data.Description = itemDto.Description;
                            data.Total = itemDto.Total;
                            data.GrandTotal = itemDto.GrandTotal;
                            data.TotalVAT = itemDto.TotalVAT;
                            data.PaymentModeId = itemDto.PaymentModeId;
                            data.IsActive = true;


                            context.Entry(data).Property(x => x.Date).IsModified = true;
                            context.Entry(data).Property(x => x.Description).IsModified = true;
                            context.Entry(data).Property(x => x.Total).IsModified = true;
                            context.Entry(data).Property(x => x.TotalVAT).IsModified = true;
                            context.Entry(data).Property(x => x.GrandTotal).IsModified = true;
                            context.Entry(data).Property(x => x.IsActive).IsModified = true;
                            context.Entry(data).Property(x => x.PaymentModeId).IsModified = true;

                            foreach (var det in itemDto.CompanyExpenseDetails)
                            {
                                if (det.Description != "" && det.Price > 0)
                                {
                                    if (det.Id > 0)
                                    {
                                        var olddet = context.CompanyExpenseDetails.FirstOrDefault(X => X.Id == det.Id);

                                        olddet.Description = det.Description;
                                        olddet.Quantity = det.Quantity;
                                        olddet.Price = det.Price;
                                        olddet.SubTotal = det.SubTotal;
                                        olddet.Total = det.Total;
                                        olddet.VATPrice = det.VATPrice;
                                        olddet.GrandTotal = det.GrandTotal;
                                        olddet.CompanyExpenseId = data.Id;
                                        olddet.ExpenseId = det.ExpenseId;
                                        olddet.IsActive = true;


                                        context.Entry(olddet).Property(x => x.Description).IsModified = true;
                                        context.Entry(olddet).Property(x => x.Quantity).IsModified = true;
                                        context.Entry(olddet).Property(x => x.Price).IsModified = true;
                                        context.Entry(olddet).Property(x => x.SubTotal).IsModified = true;
                                        context.Entry(olddet).Property(x => x.Total).IsModified = true;
                                        context.Entry(olddet).Property(x => x.VATPrice).IsModified = true;
                                        context.Entry(olddet).Property(x => x.GrandTotal).IsModified = true;
                                        context.Entry(olddet).Property(x => x.CompanyExpenseId).IsModified = true;
                                        context.Entry(olddet).Property(x => x.IsActive).IsModified = true;
                                        context.Entry(olddet).Property(x => x.ExpenseId).IsModified = true;
                                    }
                                    else
                                    {
                                        CompanyExpenseDetails olddet = new CompanyExpenseDetails();

                                        olddet.Description = det.Description;
                                        olddet.Quantity = det.Quantity;
                                        olddet.Price = det.Price;
                                        olddet.SubTotal = det.SubTotal;
                                        olddet.Total = det.Total;
                                        olddet.ExpenseId = det.ExpenseId;
                                        olddet.VATPrice = det.VATPrice;
                                        olddet.GrandTotal = det.GrandTotal;
                                        olddet.CompanyExpenseId = data.Id;
                                        olddet.IsActive = true;

                                        context.CompanyExpenseDetails.Add(olddet);
                                    }
                                }

                            }

                            context.SaveChanges();
                            return true;

                        }
                        return false;
                    }
                    else
                    {
                        CompanyExpense data = new CompanyExpense();

                        data.Date = itemDto.Date;
                        data.Description = itemDto.Description;
                        data.Total = itemDto.Total;
                        data.GrandTotal = itemDto.GrandTotal;
                        data.TotalVAT = itemDto.TotalVAT;
                        data.PaymentModeId = itemDto.PaymentModeId;
                        data.IsActive = true;

                        context.CompanyExpenses.Add(data);
                        context.SaveChanges();

                        foreach (var det in itemDto.CompanyExpenseDetails)
                        {

                            if (det.Description != "" && det.Price > 0)
                            {
                                CompanyExpenseDetails olddet = new CompanyExpenseDetails();

                                olddet.Description = det.Description;
                                olddet.Quantity = det.Quantity;
                                olddet.Price = det.Price;
                                olddet.SubTotal = det.SubTotal;
                                olddet.Total = det.Total;
                                olddet.ExpenseId = det.ExpenseId;
                                olddet.VATPrice = det.VATPrice;
                                olddet.GrandTotal = det.GrandTotal;
                                olddet.CompanyExpenseId = data.Id;
                                olddet.IsActive = true;

                                context.CompanyExpenseDetails.Add(olddet);
                            }

                        }

                        context.SaveChanges();
                        return true;

                    }
                }

            }
            return false;
        }

        [HttpGet]
        [Route("DeleteCompanyExpense/{id}")]
        public bool DeleteCompanyExpense(long id)
        {
            using (EAharaDB context = new EAharaDB())
            {
                if (id != 0)
                {
                    var Delete = context.CompanyExpenses.FirstOrDefault(x => x.Id == id);
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
        [Route("CompanyExpenseById/{id}")]
        public CompanyExpenseDto CompanyExpenseById(long id)
        {
            CompanyExpenseDto ItemDto = new CompanyExpenseDto();
            using (EAharaDB context = new EAharaDB())
            {
                var acctype = context.CompanyExpenses.FirstOrDefault(x => x.IsActive == true && x.Id == id);

                if (acctype != null)
                {
                    ItemDto.Id = acctype.Id;
                    ItemDto.Description = acctype.Description;
                    ItemDto.Date = acctype.Date;
                    ItemDto.Total = acctype.Total;
                    ItemDto.TotalVAT = acctype.TotalVAT;
                    ItemDto.GrandTotal = acctype.GrandTotal;
                    ItemDto.IsActive = acctype.IsActive;
                    ItemDto.PaymentModeId = acctype.PaymentModeId;
                    ItemDto.PaymentMode = new PaymentModeDto
                    {
                        Id = acctype.PaymentMode.Id,
                        Name = acctype.PaymentMode.Name,
                    };
                    ItemDto.CompanyExpenseDetails = acctype.CompanyExpenseDetails.Where(x => x.IsActive)
                        .Select(x => new CompanyExpenseDetailsDto
                        {
                            Id = x.Id,
                            Description = x.Description,
                            Quantity = x.Quantity,
                            Price = x.Price,
                            Total = x.Total,
                            SubTotal = x.SubTotal,
                            VATPrice = x.VATPrice,
                            GrandTotal = x.GrandTotal,
                            CompanyExpenseId = x.CompanyExpenseId,
                            ExpenseId = x.ExpenseId,
                            Expense = new ExpenseDto
                            {
                                Name = x.Expense != null ? x.Expense.Name : "",
                                Id = x.Expense != null ? x.Expense.Id : 0
                            }
                        }).ToList();

                }
            }
            return ItemDto;
        }

        [HttpPost]
        [Route("CompanyexpenseInView")]
        public DataSourceResult CompanyexpenseInView(KendoFilterRequestDto Request)
        {
            using (EAharaDB context = new EAharaDB())
            {
                var expenses = context.CompanyExpenses.Where(x => x.IsActive);

                if (Request.FromDate != null && Request.ToDate != null)
                {
                    expenses = expenses.Where(x => (DbFunctions.TruncateTime(x.Date)) >= (DbFunctions.TruncateTime(Request.FromDate)) && (DbFunctions.TruncateTime(x.Date)) <= (DbFunctions.TruncateTime(Request.ToDate)));
                }

                var dataSourceResult = expenses
                    .Select(x => new CompanyExpenseDto
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Date = x.Date,
                        Total = x.Total,
                        TotalVAT = x.TotalVAT,
                        GrandTotal = x.GrandTotal,
                        IsActive = x.IsActive,
                        PaymentModeId = x.PaymentModeId,
                        PaymentMode = new PaymentModeDto
                        {
                            Id = x.PaymentMode.Id,
                            Name = x.PaymentMode.Name,
                        },

                        CompanyExpenseDetails = x.CompanyExpenseDetails.Where(y => y.IsActive)
                        .Select(y => new CompanyExpenseDetailsDto
                        {
                            Id = y.Id,
                            Description = y.Description,
                            Quantity = y.Quantity,
                            Price = y.Price,
                            Total = y.Total,
                            SubTotal = y.SubTotal,
                            VATPrice = y.VATPrice,
                            GrandTotal = y.GrandTotal,
                            CompanyExpenseId = y.CompanyExpenseId,
                        }).ToList(),

                }).OrderByDescending(x => x.Id).ToDataSourceResult(Request);

                DataSourceResult kendoResponseDto = new DataSourceResult();
                kendoResponseDto.Data = dataSourceResult.Data;
                kendoResponseDto.Aggregates = dataSourceResult.Aggregates;
                kendoResponseDto.Total = dataSourceResult.Total;
                return kendoResponseDto;
            }
        }

    }
}
