using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class CompanyExpenseDetailsDto
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public float Total { get; set; }
        public float VATPrice { get; set; }
        public float SubTotal { get; set; }
        public float GrandTotal { get; set; }
        public bool IsActive { get; set; }

        public long ExpenseId { get; set; }
        public virtual ExpenseDto Expense { get; set; }

        public long CompanyExpenseId { get; set; }
        public virtual CompanyExpenseDto CompanyExpense { get; set; }
    }
}