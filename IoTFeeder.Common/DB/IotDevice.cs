using System;
using System.Collections.Generic;

namespace IoTFeeder.Common.DB
{
    public partial class IotDevice
    {
        public IotDevice()
        {
            IotDeviceProperties = new HashSet<IotDeviceProperty>();
        }

        public int Id { get; set; }
        public string DeviceName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsActive { get; set; }
        public int? Frequency { get; set; }
        public bool? FrequencyType { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }

        public virtual ICollection<IotDeviceProperty> IotDeviceProperties { get; set; }
    }
}
