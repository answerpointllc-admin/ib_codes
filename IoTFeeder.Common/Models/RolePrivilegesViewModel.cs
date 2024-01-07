using IoTFeeder.Common.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFeeder.Common.Models
{
    public class RolePrivilegesViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [Display(Name = "Role")]
        public int RoleId { get; set; }
        public SelectList? Roles { get; set; }
        public int MenuItemId { get; set; }
        public bool? View { get; set; }
        public bool? Add { get; set; }
        public bool? Edit { get; set; }
        public bool? Delete { get; set; }
        public bool? Detail { get; set; }
        public bool IsActive { get; set; }
        public MenuItemViewModel? MenuItem { get; set; }
        public CommonMessagesViewModel? ModuleName { get; set; }
    }
}
