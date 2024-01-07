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
    public class IoTDevicePropertyViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [Display(Name = "Device Name")]
        public int IoTDeviceId { get; set; }
        public string DeviceName { get; set; }
        public string StrProperty { get; set; }
        public SelectList? SelectedDevice { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [Display(Name = "Property Name")]
        public string PropertyName { get; set; }
        public string MaxLength { get; set; }
        public float? MinValue { get; set; }
        public decimal? MininmumValue { get; set; }
        public float? MaxValue { get; set; }
        public decimal? MaximumValue { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [Display(Name = "Data Type")]
        public int DataTypeId { get; set; }
        public string DataType { get; set; }
        public SelectList? SelectDataType { get; set; }
        public bool IsActive { get; set; }
        public string? Active { get; set; }
        public List<IoTDevicePropertyViewModel> ioTDeviceProperties { get; set; }
    }
}
