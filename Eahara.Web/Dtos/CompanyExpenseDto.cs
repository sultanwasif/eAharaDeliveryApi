using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class CompanyExpenseDto
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public float Total { get; set; }
        public float TotalVAT { get; set; }
        public float GrandTotal { get; set; }
        public bool IsActive { get; set; }

        public long PaymentModeId { get; set; }
        public virtual PaymentModeDto PaymentMode { get; set; }

        public virtual ICollection<CompanyExpenseDetailsDto> CompanyExpenseDetails { get; set; }
    }
}