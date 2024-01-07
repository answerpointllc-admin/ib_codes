using System.Linq;
using System.Collections.Generic;
using IoTFeeder.Common.Models;

namespace IoTFeeder.Common.Interfaces
{
    public interface ICommonSettings
    {
        CommonSettingsViewModel GetCommonSetting();
    }
}
