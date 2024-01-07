using System.Linq;
using System.Collections.Generic;
using IoTFeeder.Common.Models;

namespace IoTFeeder.Common.Interfaces
{
    public interface IIoTDevice
    {
        IQueryable<IoTDeviceViewModel> GetDeviceList(string searchValue);
        void SaveChanges(IoTDeviceViewModel IoTDeviceViewModel);
        IoTDeviceViewModel GetDeviceDetailById(int deviceID);
        void DeleteDevices(int[] ids);
        List<DropDownBindViewModel> GetDeviceDropdownList(int deviceId);
        List<DropDownBindViewModel> GetDataTypeDropdownList(int dataTypeId);

        //Device Frequency
        void SaveFrequence(IoTDeviceViewModel ioTDeviceViewModel);
        IQueryable<IoTDeviceViewModel> GetDeviceFrequencyList(string searchValue);
        IoTDeviceViewModel GetDeviceFrequencyDetailById(int deviceID);
        List<DropDownBindViewModel> BindDeviceDropdownList(bool isEditable);
        bool DeleteDeviceFrequency(int[] chkDelete);
        List<DropDownBindViewModel> BindIoTDeviceDropdownList(bool isEditable);
        List<IoTDeviceViewModel> GetDevices();
    }
}
