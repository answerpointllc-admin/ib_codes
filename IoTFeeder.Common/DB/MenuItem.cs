using System;
using System.Collections.Generic;

namespace IoTFeeder.Common.DB
{
    public partial class MenuItem
    {
        public int Id { get; set; }
        public string MenuItemName { get; set; } = null!;
        public string? MenuItemCotroller { get; set; }
        public string? MenuItemView { get; set; }
        public int SortOrder { get; set; }
        public int? ParentId { get; set; }
        public bool IsActive { get; set; }
        public string? ImageName { get; set; }
    }
}
