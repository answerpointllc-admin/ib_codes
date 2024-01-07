using IoTFeeder.Common.Common;
using IoTFeeder.Common.DB;
using IoTFeeder.Common.Interfaces;
using IoTFeeder.Common.Models;
using System.Collections.Generic;
using System.Linq;

namespace IoTFeeder.Common.Repositories
{
    public class IoTDevicePropertyRepository : IIoTDeviceProperty
    {
        #region Member Declaration
        private readonly IotDataFeederContext _Context;
        public IoTDevicePropertyRepository(IotDataFeederContext context)
        {
            this._Context = context;
        }
        #endregion

        #region Get IoTDevice Properties Data for Grid
        public IQueryable<IoTDevicePropertyViewModel> GetDevicePropertyList(string searchValue)
        {
            var result = _Context.IotDeviceProperties.Where(e =>(searchValue == null || e.IotDevice.DeviceName.Contains(searchValue))).GroupBy(e => e.IotDeviceId).Select(e =>
            new IoTDevicePropertyViewModel
            {
                Id = e.Key,
                DeviceName = e.FirstOrDefault().IotDevice.DeviceName,
            }).AsQueryable();

            return result;
        }
        #endregion

        #region Save & Update IoTDevice Properties
        public void SaveChanges(IoTDevicePropertyViewModel ioTDevicePropertyViewModel)
        {

            foreach (var item in ioTDevicePropertyViewModel.ioTDeviceProperties)
            {
                IotDeviceProperty? objTblIoTDeviceProperty = new IotDeviceProperty();
                objTblIoTDeviceProperty.IotDeviceId = ioTDevicePropertyViewModel.IoTDeviceId;
                objTblIoTDeviceProperty.PropertyName = (item.PropertyName ?? string.Empty).Trim();

                objTblIoTDeviceProperty.DataType = item.DataTypeId;
                if (item.DataTypeId == 3)
                {
                    objTblIoTDeviceProperty.MaxLength = item.MinValue != null ? item.MinValue.ToString() : null;
                }
                else
                {
                    objTblIoTDeviceProperty.MinValue = item.MinValue != null ? (decimal)item.MinValue : null;
                }
                objTblIoTDeviceProperty.MaxValue = item.MaxValue != null ? (decimal)item.MaxValue : null;
                objTblIoTDeviceProperty.IsActive = ioTDevicePropertyViewModel.IsActive;

                _Context.IotDeviceProperties.Add(objTblIoTDeviceProperty);
            }
            if (_Context.SaveChanges() > 0)
            {
                Console.WriteLine("sss");
            }
            
        }
        #endregion


        #region Get IoTDevice Properties Details By deviceID
        public List<IoTDevicePropertyViewModel> GetDevicesPropertyDetailById(int propertyID)
        {
            List<IoTDevicePropertyViewModel> Devicedetail = (from p in _Context.IotDeviceProperties
                                                             where p.IotDeviceId == propertyID
                                                             select new IoTDevicePropertyViewModel
                                                             {
                                                                 Id = p.Id,
                                                                 PropertyName = p.PropertyName != null ? p.PropertyName : string.Empty,
                                                                 IoTDeviceId = p.IotDeviceId,
                                                                 DeviceName = p.IotDevice.DeviceName != null ? p.IotDevice.DeviceName : string.Empty,
                                                                 DataTypeId = p.DataType,
                                                                 DataType = p.DataTypeNavigation.EnumValue != null ? p.DataTypeNavigation.EnumValue.ToString() : string.Empty,
                                                                 MinValue = p.MinValue.HasValue ? (float)p.MinValue : null,
                                                                 MaxValue = p.MaxValue.HasValue ? (float)p.MaxValue : null,
                                                                 MaxLength = p.MaxLength != null ? p.MaxLength : string.Empty,
                                                                 IsActive = p.IsActive,
                                                                 Active = p.IsActive ? GlobalCode.ActiveText : GlobalCode.InActiveText,
                                                             }).ToList();
            return Devicedetail;
        }
        #endregion

        #region Delete IoTDevice properties
        public void DeleteDeviceProperties(int[] ids)
        {
            _Context.IotDeviceProperties.RemoveRange(_Context.IotDeviceProperties.Where(r => ids.Contains(r.IotDeviceId)).AsEnumerable());
            _Context.SaveChanges();

            _Context.IotDevices.RemoveRange(_Context.IotDevices.Where(r => ids.Contains(r.Id)).AsEnumerable());
            _Context.SaveChanges();

        }
        #endregion       

        //#region Save & Update IoTDevice
        //public void SaveUpdate(IoTDevicePropertyViewModel ioTDevicePropertyViewModel)
        //{
        //    IotDeviceProperty? objTblIoTDeviceProperty = new IotDeviceProperty();
        //    //if (ioTDevicePropertyViewModel.Id > 0)
        //    //{
        //    //    objTblIoTDeviceProperty = _Context.IotDeviceProperties.Where(r => r.Id == ioTDevicePropertyViewModel.Id).FirstOrDefault();
        //    //}
        //    objTblIoTDeviceProperty.IotDeviceId = ioTDevicePropertyViewModel.IoTDeviceId;
        //    objTblIoTDeviceProperty.PropertyName = (ioTDevicePropertyViewModel.PropertyName ?? string.Empty).Trim();
        //    objTblIoTDeviceProperty.DataType = ioTDevicePropertyViewModel.DataTypeId;
        //    objTblIoTDeviceProperty.MinValue = ioTDevicePropertyViewModel.MininmumValue != null ? (decimal)ioTDevicePropertyViewModel.MininmumValue : null;
        //    objTblIoTDeviceProperty.MaxValue = ioTDevicePropertyViewModel.MaximumValue != null ? (decimal)ioTDevicePropertyViewModel.MaximumValue : null;
        //    objTblIoTDeviceProperty.MaxLength = ioTDevicePropertyViewModel.MaxLength;
        //    objTblIoTDeviceProperty.IsActive = ioTDevicePropertyViewModel.IsActive;
        //    if (ioTDevicePropertyViewModel.Id == 0)
        //    {
        //        _Context.IotDeviceProperties.Add(objTblIoTDeviceProperty);
        //    }
        //    _Context.SaveChanges();
        //}
        //#endregion

        #region Get IoTDevice Details By deviceID
        public IoTDevicePropertyViewModel GetDevicePropertyDetailById(int propertyID)
        {
            IoTDevicePropertyViewModel? Devicedetail = (from p in _Context.IotDeviceProperties
                                                        where p.IotDeviceId == propertyID
                                                        select new IoTDevicePropertyViewModel
                                                        {
                                                            Id = p.Id,
                                                            PropertyName = p.PropertyName != null ? p.PropertyName : string.Empty,
                                                            IoTDeviceId = p.IotDeviceId,
                                                            DeviceName = p.IotDevice.DeviceName != null ? p.IotDevice.DeviceName : string.Empty,
                                                            DataTypeId = p.DataType,
                                                            DataType = p.DataTypeNavigation.EnumValue != null ? p.DataTypeNavigation.EnumValue.ToString() : string.Empty,
                                                            MininmumValue = p.MinValue,
                                                            MaximumValue = p.MaxValue,
                                                            MaxLength = p.MaxLength,
                                                            IsActive = p.IsActive,
                                                            Active = p.IsActive ? GlobalCode.ActiveText : GlobalCode.InActiveText,
                                                        }).FirstOrDefault();
            return Devicedetail;
        }
        #endregion

        #region Delete Properties Only
        public void DeleteProprties(int[] ids)
        {
            _Context.IotDeviceProperties.RemoveRange(_Context.IotDeviceProperties.Where(r => ids.Contains(r.IotDeviceId)).AsEnumerable());
            _Context.SaveChanges();
        }
        #endregion

        #region Get Devicename from Id
        public List<IoTDeviceViewModel> GetDeviceNameFromId(int[] ids)
        {
            List<IoTDeviceViewModel>? ioTDeviceViewModel = (from e in _Context.IotDevices
                                                            where ids.Contains(e.Id)
                                                            select new IoTDeviceViewModel
                                                            {
                                                                Id = e.Id,
                                                                DeviceName = e.DeviceName
                                                            }).ToList();
            return ioTDeviceViewModel;

        }
        #endregion
    }
}
