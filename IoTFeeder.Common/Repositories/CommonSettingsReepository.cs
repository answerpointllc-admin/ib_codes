using IoTFeeder.Common.Common;
using IoTFeeder.Common.DB;
using IoTFeeder.Common.Interfaces;
using IoTFeeder.Common.Models;
using System.Collections.Generic;
using System.Linq;

namespace IoTFeeder.Common.Repositories
{
    public class CommonSettingsReepository : ICommonSettings
    {
        #region Member Declaration
        private readonly IotDataFeederContext _Context;
        public CommonSettingsReepository(IotDataFeederContext context)
        {
            this._Context = context;
        }
        #endregion

        #region Get Common Setting
        public CommonSettingsViewModel GetCommonSetting()
        {
            return _Context.CommonSettings.Select(x => new CommonSettingsViewModel
            {
                KustoUri = x.KustoUri,
                ClientId = x.ClientId,
                ClientSecret = x.ClientSecret,
                TenantId = x.TenantId,
                DatabaseName = x.DatabaseName,
            }).FirstOrDefault();
        }
        #endregion
    }
}
