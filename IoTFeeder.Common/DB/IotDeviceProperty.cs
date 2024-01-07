using System;
using System.Collections.Generic;

namespace IoTFeeder.Common.DB
{
    public partial class IotDeviceProperty
    {
        public int Id { get; set; }
        public int IotDeviceId { get; set; }
        public string PropertyName { get; set; } = null!;
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public int DataType { get; set; }
        public bool IsActive { get; set; }
        public string? MaxLength { get; set; }

        public virtual Enum DataTypeNavigation { get; set; } = null!;
        public virtual IotDevice IotDevice { get; set; } = null!;
    }
}
