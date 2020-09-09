using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class FilterDto
    {
        public long Id { get; set; }
        public long CategoryId { get; set; }
        public bool Preference { get; set; }
        public bool Preference2 { get; set; }
        public int Pagenation { get; set; }
        public string Keyword { get; set; }
        public List<long> ShopsCategories { get; set; }
        public List<long> ItemCategories { get; set; }

        public string lng { get; set; }
        public string lat { get; set; }

        public long id { get; set; }
        public long sid { get; set; }
        public string Remarks { get; set; }
        public string CancelRemarks { get; set; }

        public int New { get; set; }
        public int Packed { get; set; }
        public int Cancelled { get; set; }
        public int Delivered { get; set; }
        public int Approved { get; set; }
        public long LocationId { get; set; }
    }
}