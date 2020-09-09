using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
   public class CompanyExpenseDetails
    {
        public long Id { get; set; }
        [StringLength(600)]
        public string Description { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }  
        public float Total { get; set; }  
        public float VATPrice { get; set; }
        public float SubTotal { get; set; }
        public float GrandTotal { get; set; }
        public bool IsActive { get; set; }

        public long ExpenseId { get; set; }
        public virtual Expense Expense { get; set; }

        public long CompanyExpenseId { get; set; }
        public virtual CompanyExpense CompanyExpense { get; set; }

    }
}
