using IoTFeeder.Common.Common;
using IoTFeeder.Common.DB;
using IoTFeeder.Common.Interfaces;
using IoTFeeder.Common.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace IoTFeeder.Common.Repositories
{
    public class IoTDeviceRepository : IIoTDevice
    {
        #region Member Declaration
        private readonly IotDataFeederContext _Context;
        public IoTDeviceRepository(IotDataFeederContext context)
        {
            this._Context = context;
        }
        #endregion

        #region Get IoTDevice Data for Grid
        public IQueryable<IoTDeviceViewModel> GetDeviceList(string searchValue)
        {
            var result = (from p in _Context.IotDevices
                          where (!string.IsNullOrEmpty(searchValue) ? (p.DeviceName.Contains(searchValue)) : true) ||
                          (!string.IsNullOrEmpty(searchValue) ? (p.IsActive ? GlobalCode.ActiveText : GlobalCode.InActiveText).Contains(searchValue) : true)

                          select new IoTDeviceViewModel
                          {
                              Id = p.Id,
                              DeviceName = p.DeviceName != null ? p.DeviceName : string.Empty,
                              Description = p.Description != null ? p.Description : string.Empty,
                              IsActive = p.IsActive,
                              Active = p.IsActive ? GlobalCode.ActiveText : GlobalCode.InActiveText,
                          });

            return result;
        }
        #endregion

        #region Save & Update IoT Device
        public void SaveChanges(IoTDeviceViewModel IoTDeviceViewModel)
        {
            IotDevice? objTblIoTDevice = new IotDevice();
            if (IoTDeviceViewModel.Id > 0)
            {
                objTblIoTDevice = _Context.IotDevices.Where(r => r.Id == IoTDeviceViewModel.Id).FirstOrDefault();
            }
            objTblIoTDevice.DeviceName = (IoTDeviceViewModel.DeviceName ?? string.Empty).Trim();
            objTblIoTDevice.Description = IoTDeviceViewModel.Description;
            objTblIoTDevice.IsActive = IoTDeviceViewModel.IsActive;
            if (IoTDeviceViewModel.Id == 0)
            {
                _Context.IotDevices.Add(objTblIoTDevice);
            }
            _Context.SaveChanges();
        }
        #endregion

        #region Get IoTDevice Details By deviceID
        public IoTDeviceViewModel GetDeviceDetailById(int deviceID)
        {
            var Devicedetail = (from p in _Context.IotDevices
                                where p.Id == deviceID
                                select new IoTDeviceViewModel
                                {
                                    Id = p.Id,
                                    DeviceName = p.DeviceName != null ? p.DeviceName : string.Empty,
                                    Description = p.Description != null ? p.Description : string.Empty,
                                    IsActive = p.IsActive,
                                    Active = p.IsActive ? GlobalCode.ActiveText : GlobalCode.InActiveText,
                                    ioTDeviceProperties = p.IotDeviceProperties.Select(e => new IoTDevicePropertyViewModel
                                    {
                                        Id = e.Id,
                                        PropertyName = e.PropertyName,
                                        DataTypeId = e.DataType,
                                        MininmumValue = e.MinValue,
                                        MaximumValue = e.MaxValue,
                                        MaxLength = e.MaxLength != null ? e.MaxLength : "",
                                    }).ToList(),
                                }).FirstOrDefault();
            return Devicedetail;
        }
        #endregion

        #region GetDevice details
        public List<IoTDeviceViewModel> GetDevices()
        {
            List<IoTDeviceViewModel> list = (from d in _Context.IotDevices
                                             where
                                                d.IotDeviceProperties != null &&
                                                d.IotDeviceProperties.Count > 0 &&
                                                d.IsActive == true &&
                                                d.FrequencyType.HasValue
                                             select new IoTDeviceViewModel
                                             {
                                                 Id = d.Id,
                                                 DeviceName = d.DeviceName != null ? d.DeviceName : string.Empty,
                                                 FrequencyType = d.FrequencyType.HasValue ? d.FrequencyType.Value : false,
                                                 MinValue = d.MinValue,
                                                 MaxValue = d.MaxValue,
                                                 Frequency = d.Frequency,
                                                 ioTDeviceProperties = d.IotDeviceProperties.Select(e => new IoTDevicePropertyViewModel
                                                 {
                                                     Id = e.Id,
                                                     PropertyName = e.PropertyName,
                                                     DataTypeId = e.DataType,
                                                     MininmumValue = e.MinValue,
                                                     MaximumValue = e.MaxValue,
                                                     MaxLength = e.MaxLength != null ? e.MaxLength : "",
                                                 }).ToList(),
                                             }).ToList();
            return list;
        }
        #endregion

        #region Delete IoTDevices
        public void DeleteDevices(int[] ids)
        {
            _Context.IotDeviceProperties.RemoveRange(_Context.IotDeviceProperties.Where(r => ids.Contains(r.IotDeviceId)).AsEnumerable());
            _Context.SaveChanges();

            _Context.IotDevices.RemoveRange(_Context.IotDevices.Where(r => ids.Contains(r.Id)).AsEnumerable());
            _Context.SaveChanges();
        }
        #endregion       

        #region Get Device Dropdown List
        public List<DropDownBindViewModel> GetDeviceDropdownList(int deviceId)
        {
            List<DropDownBindViewModel> objDeviceList = new List<DropDownBindViewModel>();
            if (deviceId == 0)
            {
                objDeviceList = _Context.IotDevices.Where(e => e.IsActive).Select(e => new DropDownBindViewModel { value = e.Id, name = e.DeviceName }).ToList();
            }
            else
            {
                objDeviceList = _Context.IotDeviceProperties.Where(e => e.IotDeviceId == deviceId).Select(e => new DropDownBindViewModel { value = e.IotDeviceId, name = e.IotDevice.DeviceName }).ToList();
            }
            return objDeviceList;
        }
        #endregion

        #region DataType DropDown List
        public List<DropDownBindViewModel> GetDataTypeDropdownList(int dataTypeId)
        {
            List<DropDownBindViewModel> objDataTypeList = new List<DropDownBindViewModel>();
            if (dataTypeId == 0)
            {
                objDataTypeList = _Context.Enums.Where(e => e.IsActive).Select(e => new DropDownBindViewModel { value = e.Id, name = e.EnumValue }).ToList();
            }
            else
            {
                objDataTypeList = _Context.IotDeviceProperties.Where(e => e.DataType == dataTypeId).Select(e => new DropDownBindViewModel { value = e.DataType, name = e.DataTypeNavigation.EnumValue }).ToList();
            }
            return objDataTypeList;
        }
        #endregion

        //Device Frequency
        #region Frequency grid binding
        public IQueryable<IoTDeviceViewModel> GetDeviceFrequencyList(string searchValue)
        {
            var result = (from p in _Context.IotDevices
                          where (
                          (!string.IsNullOrEmpty(searchValue) ? (p.DeviceName.Contains(searchValue)) : true) ||
                          (!string.IsNullOrEmpty(searchValue) ? (p.MinValue.ToString().Contains(searchValue)) : true) ||
                          (!string.IsNullOrEmpty(searchValue) ? (p.MaxValue.ToString().Contains(searchValue)) : true) ||
                          (!string.IsNullOrEmpty(searchValue) ? (p.Frequency.ToString().Contains(searchValue)) : true) ||
                          (!string.IsNullOrEmpty(searchValue) ? (p.IsActive ? GlobalCode.ActiveText : GlobalCode.InActiveText).Contains(searchValue) : true) ||
                          (!string.IsNullOrEmpty(searchValue) ? (p.FrequencyType.HasValue && p.FrequencyType.Value ? GlobalCode.FrequencyRandom : GlobalCode.FrequencyFix).Contains(searchValue) : true)
                          )
                          && (p.Frequency != null || p.MinValue != null || p.MaxValue != null)

                          select new IoTDeviceViewModel
                          {
                              Id = p.Id,
                              DeviceName = p.DeviceName != null ? p.DeviceName : string.Empty,
                              Frequency = p.Frequency != null ? p.Frequency : null,
                              FrequencyType = p.FrequencyType.HasValue ? p.FrequencyType.Value : false,
                              FrequencyTypeText = p.FrequencyType.HasValue && p.FrequencyType.Value ? GlobalCode.FrequencyRandom : GlobalCode.FrequencyFix,
                              MinValue = p.MinValue != null ? p.MinValue : null,
                              MaxValue = p.MaxValue != null ? p.MaxValue : null,
                              IsActive = p.IsActive,
                              Active = p.IsActive ? GlobalCode.ActiveText : GlobalCode.InActiveText,
                          });

            return result;
        }
        #endregion

        #region Add Update Frequency
        public void SaveFrequence(IoTDeviceViewModel ioTDeviceViewModel)
        {
            IotDevice? objTblIoTDevice = new IotDevice();
            if (ioTDeviceViewModel.Id > 0)
            {
                objTblIoTDevice = _Context.IotDevices.Where(r => r.Id == ioTDeviceViewModel.Id).FirstOrDefault();
            }

            objTblIoTDevice.Frequency = ioTDeviceViewModel.Frequency;
            objTblIoTDevice.FrequencyType = ioTDeviceViewModel.FrequencyType;
            objTblIoTDevice.MinValue = ioTDeviceViewModel.MinValue;
            objTblIoTDevice.MaxValue = ioTDeviceViewModel.MaxValue;
            objTblIoTDevice.IsActive = ioTDeviceViewModel.IsActive;
            if (ioTDeviceViewModel.Id == 0)
            {
                _Context.IotDevices.Add(objTblIoTDevice);
            }
            _Context.SaveChanges();
        }
        #endregion

        #region Get Frequency Details By deviceID
        public IoTDeviceViewModel GetDeviceFrequencyDetailById(int deviceID)
        {
            var Devicedetail = (from p in _Context.IotDevices
                                where p.Id == deviceID && (p.Frequency != null || p.MinValue != null || p.MaxValue != null)
                                select new IoTDeviceViewModel
                                {
                                    Id = p.Id,
                                    DeviceName = p.DeviceName != null ? p.DeviceName : string.Empty,
                                    Frequency = p.Frequency != null ? p.Frequency : null,
                                    FrequencyType = p.FrequencyType.HasValue ? p.FrequencyType.Value : false,
                                    FrequencyTypeText = p.FrequencyType.HasValue && p.FrequencyType.Value ? GlobalCode.FrequencyRandom : GlobalCode.FrequencyFix,
                                    MinValue = p.MinValue != null ? p.MinValue : null,
                                    MaxValue = p.MaxValue != null ? p.MaxValue : null,
                                    IsActive = p.IsActive,
                                    Active = p.IsActive ? GlobalCode.ActiveText : GlobalCode.InActiveText,
                                }).FirstOrDefault();
            return Devicedetail;
        }
        #endregion

        #region Delete Frequency
        public bool DeleteDeviceFrequency(int[] chkDelete)
        {
            List<IotDevice> iotDevices = _Context.IotDevices.Where(r => chkDelete.Contains(r.Id)).ToList();
            try
            {
                foreach (var item in iotDevices)
                {
                    item.Frequency = null;
                    item.MinValue = null;
                    item.MaxValue = null;

                }
                _Context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Bind Device Dropdown List
        public List<DropDownBindViewModel> BindDeviceDropdownList(bool isEditable)
        {
            var data = _Context.IotDevices.Where(e =>(isEditable == false ? e.Frequency == null && e.MinValue == null && e.MaxValue == null : true)).Select(e => new DropDownBindViewModel { value = e.Id, name = e.DeviceName }).ToList();
            return _Context.IotDevices.Where(e =>(isEditable == false ? e.Frequency == null && e.MinValue == null && e.MaxValue == null : true)).Select(e => new DropDownBindViewModel { value = e.Id, name = e.DeviceName }).ToList();
        }
        #endregion

        #region Bind Device dropdown for properties
        public List<DropDownBindViewModel> BindIoTDeviceDropdownList(bool isEditable)
        {
            var data = _Context.IotDevices.Where(e =>(isEditable == false ? (e.IotDeviceProperties == null || (e.IotDeviceProperties != null && e.IotDeviceProperties.Count == 0)) : true)).Select(e => new DropDownBindViewModel { value = e.Id, name = e.DeviceName }).ToList();
            return data;
        }
        #endregion
    }
}