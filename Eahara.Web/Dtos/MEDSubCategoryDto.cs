using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class MEDSubCategoryDto
    {
        public long Id { get; set; }
        public long MEDCategoryId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; }

        public virtual MEDCategoryDto MEDCategory { get; set; }
    }
}