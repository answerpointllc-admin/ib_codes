using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFeeder.Common.Models
{
    public class MenuItemViewModel
    {
        public int Id { get; set; }
        public string MenuItemName { get; set; }
        public string MenuItemController { get; set; }
        public string MenuItemView { get; set; }
        public int SortOrder { get; set; }
        public int? ParentId { get; set; }
        public bool IsActive { get; set; }
        public string ImageName { get; set; }
    }
}
