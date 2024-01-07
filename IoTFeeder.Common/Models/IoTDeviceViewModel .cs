using IoTFeeder.Common.Common;
using IoTFeeder.Common.Helper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTFeeder.Common.Models
{
    public class IoTDeviceViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [Display(Name = "IoT Device Name")]
        public int Id { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "MaxAllowedLength")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [Display(Name = "Device Name")]
        public string DeviceName { get; set; }
        public string StrProperty { get; set; }
        public SelectList? SelectedDevice { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [StringLength(250, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "MaxAllowedLength")]
        [Display(Name = "Description")]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string Active { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        //[StringLength(3, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "MaxAllowedLength")]
        [RequiredIf("FrequencyType", false, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [Display(Name = "Fix Value")]
        public int? Frequency { get; set; }
        public bool FrequencyType { get; set; }
        public string? FrequencyTypeText { get; set; }

        [RequiredIf("FrequencyType", true, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [Display(Name = "Min Value")]
        public int? MinValue { get; set; }

        [RequiredIf("FrequencyType", true, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [Display(Name = "Max Value")]
        public int? MaxValue { get; set; }
        public List<IoTDevicePropertyViewModel> ioTDeviceProperties { get; set; }
        public int RecordPerSecond { get; set; } = 1;
        public bool IsBulkInsert { get; set; }
    }
}
