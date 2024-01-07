using System;
using System.Collections.Generic;

namespace IoTFeeder.Common.DB
{
    public partial class CommonSetting
    {
        public int Id { get; set; }
        public string KustoUri { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
        public string TenantId { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
    }
}
