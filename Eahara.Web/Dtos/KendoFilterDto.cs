using Kendo.DynamicLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class KendoFilterDto: DataSourceRequest
    {
        public long ShopId { get; set; }
        public long CustomerId { get; set; }
        public long EmployeeId { get; set; }
        public long MEDSubCategoryId { get; set; }
        public string Keyword { get; set; }
        public DateTime Date { get; set; }
        public float MinPrice { get; set; }
        public float MaxPrice { get; set; }
        public long MEDBrandId { get; set; }
        public long SortBy { get; set; }
    }
}