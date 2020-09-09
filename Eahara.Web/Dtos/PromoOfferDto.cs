using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class PromoOfferDto
    {
        public long Id { get; set; }
        public float Value { get; set; }
        public float MaxValue { get; set; }
        public bool IsPercentage { get; set; }
        public string Code { get; set; }
        public string Image { get; set; }
        public string Tittle { get; set; }
        public int Count { get; set; }
        public bool IsActive { get; set; }
        public bool IsSelected { get; set; }
    }
}