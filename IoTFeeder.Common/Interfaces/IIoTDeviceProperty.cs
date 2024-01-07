using System.Linq;
using System.Collections.Generic;
using IoTFeeder.Common.Models;

namespace IoTFeeder.Common.Interfaces
{
    public interface IIoTDeviceProperty
    {
        IQueryable<IoTDevicePropertyViewModel> GetDevicePropertyList(string searchValue);
        void SaveChanges(IoTDevicePropertyViewModel ioTDevicePropertyViewModel);
        List<IoTDevicePropertyViewModel> GetDevicesPropertyDetailById(int propertyID);
        void DeleteDeviceProperties(int[] ids);

        //void SaveUpdate(IoTDevicePropertyViewModel ioTDevicePropertyViewModel);
        List<IoTDeviceViewModel> GetDeviceNameFromId(int[] ids);
        IoTDevicePropertyViewModel GetDevicePropertyDetailById(int propertyID);
        void DeleteProprties(int[] ids);
    }
}
