using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eahara.Model
{
    public class CompanyExpense
    {
        public long Id { get; set; }
        [StringLength(600)]
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public float Total { get; set; }
        public float TotalVAT { get; set; }
        public float GrandTotal { get; set; }
        public bool IsActive { get; set; }
        public long PaymentModeId { get; set; }
        public virtual PaymentMode PaymentMode { get; set; }

        public virtual ICollection<CompanyExpenseDetails> CompanyExpenseDetails { get; set; }

    }
}
