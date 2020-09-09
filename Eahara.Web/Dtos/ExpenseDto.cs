using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class ExpenseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}