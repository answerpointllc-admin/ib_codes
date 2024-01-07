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
    public class CommonSettingsViewModel
    {
        public int Id { get; set; }
        public string KustoUri { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TenantId { get; set; }
        public string DatabaseName { get; set; }
    }
}
