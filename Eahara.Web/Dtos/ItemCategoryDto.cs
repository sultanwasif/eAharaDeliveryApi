using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eahara.Web.Dtos
{
    public class ItemCategoryDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; }
        public bool IsChecked { get; set; }
        public long ItemCategoryId { get; set; }
        public int Priority { get; set; }

        public List<ItemDto> Items { get; set; }
    }
}