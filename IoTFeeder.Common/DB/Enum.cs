using System;
using System.Collections.Generic;

namespace IoTFeeder.Common.DB
{
    public partial class Enum
    {
        public Enum()
        {
            IotDeviceProperties = new HashSet<IotDeviceProperty>();
        }

        public int Id { get; set; }
        public string EnumValue { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsActive { get; set; }

        public virtual ICollection<IotDeviceProperty> IotDeviceProperties { get; set; }
    }
}
