using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class DeviceDto
    {
        public long Id { get; set; }
        public string Platform { get; set; }
        public string UUId { get; set; }
        public string version { get; set; }
        public string Language { get; set; }
    }
}