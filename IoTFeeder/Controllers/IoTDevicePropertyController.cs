using System;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Linq;
using IoTFeeder.Common.Interfaces;
using IoTFeeder.Common.Common;
using IoTFeeder.Common.Models;
using IoTFeeder.Admin.CustomBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Runtime.Serialization.Json;
using IoTFeeder.Helper;
using Microsoft.Identity.Client;

namespace IoTFeeder.Admin.Controllers
{
    public class IoTDevicePropertyController : Controller
    {
        #region Members Declaration
        private readonly IIoTDevice _IoTDeviceRepository;
        private readonly IIoTDeviceProperty _IoTDevicePropertyRepository;
        private readonly ICommonSettings _commonSettingsRepository;
        private static int _TotalCount = 0;

        public IoTDevicePropertyController(IIoTDevice ioTDeviceRepository, IIoTDeviceProperty ioTDevicePropertyRepository, ICommonSettings commonSettingsRepository)
        {
            this._IoTDeviceRepository = ioTDeviceRepository;
            this._IoTDevicePropertyRepository = ioTDevicePropertyRepository;
            this._commonSettingsRepository = commonSettingsRepository;
        }
        #endregion

        #region Index
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.ModuleName = "IoT device properties";
            return View();
        }
        #endregion

        #region Ajax Binding
        public JsonResult _AjexBinding([DataSourceRequest] DataSourceRequest command, string searchValue)
        {
            var result = new DataSourceResult()
            {
                Data = GetIoTDevicePropertyGridData(command, searchValue),
                Total = _TotalCount
            };
            return Json(result);
        }

        public IEnumerable GetIoTDevicePropertyGridData([DataSourceRequest] DataSourceRequest command, string searchValue)
        {
            var result = _IoTDevicePropertyRepository.GetDevicePropertyList(searchValue);

            result = result.ApplyFiltering(command.Filters);

            _TotalCount = result.Count();

            result = result.ApplySorting(command.Groups, command.Sorts);

            result = result.ApplyPaging(command.Page, command.PageSize);

            if (command.Groups.Any())
            {
                return result.ApplyGrouping(command.Groups);
            }
            return result.ToList();
        }
        #endregion

        #region Create 
        [HttpGet]
        public ActionResult Create()
        {
            IoTDevicePropertyViewModel ioTDevicePropertyModel = new IoTDevicePropertyViewModel();
            ioTDevicePropertyModel = BindDropDown(ioTDevicePropertyModel, false);
            ioTDevicePropertyModel.IsActive = true;
            return View(ioTDevicePropertyModel);
        }

        [HttpPost]
        public ActionResult Create(IoTDevicePropertyViewModel ioTDevicePropertyModel)
        {
            ModelState.Remove("Active");
            ModelState.Remove("DeviceName");
            ModelState.Remove("DataType");
            ModelState.Remove("ioTDeviceProperties");
            ModelState.Remove("MaxLength");
            ModelState.Remove("StrProperty");
            if (ModelState.IsValid)
            {
                //string propertyName = HttpContext.Request.Form["PropertyName"];                
                //List<string> names = propertyName.Split(",").ToList();
                if (ioTDevicePropertyModel.StrProperty != null)
                {
                    using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(ioTDevicePropertyModel.StrProperty)))
                    {
                        DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(List<IoTDevicePropertyViewModel>));
                        ioTDevicePropertyModel.ioTDeviceProperties = (List<IoTDevicePropertyViewModel>)deserializer.ReadObject(ms);
                        ioTDevicePropertyModel.StrProperty = "";
                    }
                }

                string deviceName = string.Empty;
                var deviceDetail = _IoTDeviceRepository.GetDeviceDetailById(ioTDevicePropertyModel.IoTDeviceId);
                if (deviceDetail != null)
                {
                    deviceName = deviceDetail.DeviceName;
                }
                var commonSettingData = _commonSettingsRepository.GetCommonSetting();
                AzureDataExporerHelper.kustoUri = commonSettingData.KustoUri;
                AzureDataExporerHelper.clientId = commonSettingData.ClientId;
                AzureDataExporerHelper.clientSecret = commonSettingData.ClientSecret;
                AzureDataExporerHelper.tenantId = commonSettingData.TenantId;
                AzureDataExporerHelper.databaseName = commonSettingData.DatabaseName;
                var data = AzureDataExporerHelper.CreateTable(ioTDevicePropertyModel, deviceName);
                if (data)
                {
                    _IoTDevicePropertyRepository.SaveChanges(ioTDevicePropertyModel);
                    return RedirectToAction("Index", "IoTDeviceProperty", new { msg = "added" });
                }
                else
                {
                    ModelState.AddModelError("", "Failed creating needed object over ADE");
                    ioTDevicePropertyModel = BindDropDown(ioTDevicePropertyModel, true);
                    return View(ioTDevicePropertyModel);
                }
            }
            else
            {
                ioTDevicePropertyModel = BindDropDown(ioTDevicePropertyModel, true);
                return View(ioTDevicePropertyModel);
            }
        }
        #endregion

        #region Edit 
        [HttpGet]
        public ActionResult Edit(int id)
        {
            IoTDevicePropertyViewModel ioTDevicePropertyViewModel = new IoTDevicePropertyViewModel();
            var ioTDevicePropertyModel = _IoTDevicePropertyRepository.GetDevicesPropertyDetailById(id);
            if (ioTDevicePropertyModel == null)
            {
                return RedirectToAction("Index", "IoT Device Property", new { msg = "drop" });
            }
            ioTDevicePropertyViewModel = BindDropDown(ioTDevicePropertyViewModel, true);
            ioTDevicePropertyViewModel.ioTDeviceProperties = ioTDevicePropertyModel;
            ioTDevicePropertyViewModel.IoTDeviceId = ioTDevicePropertyModel.Select(e => e.IoTDeviceId).FirstOrDefault();
            ioTDevicePropertyViewModel.DeviceName = ioTDevicePropertyModel.Select(e => e.DeviceName).FirstOrDefault();
            ioTDevicePropertyViewModel.IsActive = ioTDevicePropertyModel.Select(e => e.IsActive).FirstOrDefault();


            //languageitem.PropertyName = answers;
            //languageitem.DataTypeId = dd;
            //languageitem.MinValue = rightanswer1;
            //languageitem.MaxValue = rightanswer;
            //languageitem.MaxLength = 0; // rightanswer2;


            //ioTDevicePropertyViewModel.StrProperty = 

            return View(ioTDevicePropertyViewModel);
        }

        private string GenerateJson(IoTDevicePropertyViewModel obj)
        {


            return "";
        }



        [HttpPost]
        public ActionResult Edit(IoTDevicePropertyViewModel ioTDevicePropertyModel)
        {
            ModelState.Remove("Active");
            ModelState.Remove("DeviceName");
            ModelState.Remove("DataType");
            ModelState.Remove("ioTDeviceProperties");
            ModelState.Remove("MaxLength");
            ModelState.Remove("DataTypeId");
            ModelState.Remove("PropertyName");
            if (ModelState.IsValid)
            {
                var commonSettingData = _commonSettingsRepository.GetCommonSetting();

                if (ioTDevicePropertyModel.StrProperty != null)
                {
                    using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(ioTDevicePropertyModel.StrProperty)))
                    {
                        DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(List<IoTDevicePropertyViewModel>));
                        ioTDevicePropertyModel.ioTDeviceProperties = (List<IoTDevicePropertyViewModel>)deserializer.ReadObject(ms);
                        ioTDevicePropertyModel.StrProperty = "";
                    }
                }

                #region Delete all the properties and table from ADE and also delete it from db
                int[] deviceIds = new int[] { ioTDevicePropertyModel.Id };
                var deviceNames = _IoTDevicePropertyRepository.GetDeviceNameFromId(deviceIds);
                var status = false;
                if (deviceNames != null)
                {
                    AzureDataExporerHelper.kustoUri = commonSettingData.KustoUri;
                    AzureDataExporerHelper.clientId = commonSettingData.ClientId;
                    AzureDataExporerHelper.clientSecret = commonSettingData.ClientSecret;
                    AzureDataExporerHelper.tenantId = commonSettingData.TenantId;
                    AzureDataExporerHelper.databaseName = commonSettingData.DatabaseName;
                    foreach (var item in deviceNames)
                    {
                        status = AzureDataExporerHelper.DeleteTable(item.DeviceName.Replace(" ", "_"));
                        if (status)
                        {
                            _IoTDevicePropertyRepository.DeleteProprties(deviceIds);
                        }
                    }
                }
                #endregion

                #region Recreate all the properties and table in ADE and also in db
                string deviceName = string.Empty;
                var deviceDetail = _IoTDeviceRepository.GetDeviceDetailById(ioTDevicePropertyModel.Id);
                if (deviceDetail != null)
                {
                    deviceName = deviceDetail.DeviceName;
                }
                AzureDataExporerHelper.kustoUri = commonSettingData.KustoUri;
                AzureDataExporerHelper.clientId = commonSettingData.ClientId;
                AzureDataExporerHelper.clientSecret = commonSettingData.ClientSecret;
                AzureDataExporerHelper.tenantId = commonSettingData.TenantId;
                AzureDataExporerHelper.databaseName = commonSettingData.DatabaseName;
                var data = AzureDataExporerHelper.CreateTable(ioTDevicePropertyModel, deviceName);
                if (data)
                {
                    _IoTDevicePropertyRepository.SaveChanges(ioTDevicePropertyModel);
                }
                #endregion

                return RedirectToAction("Index", "IoTDeviceProperty", new { msg = "updated" });
            }
            else
            {
                ioTDevicePropertyModel = BindDropDown(ioTDevicePropertyModel, true);
                return View(ioTDevicePropertyModel);
            }
        }
        #endregion

        #region Delete
        [HttpPost]
        public ActionResult Delete(int[] chkDelete)
        {
            try
            {
                if (chkDelete.Length > 0)
                {
                    var deviceNames = _IoTDevicePropertyRepository.GetDeviceNameFromId(chkDelete);
                    var status = false;
                    if (deviceNames != null)
                    {
                        var commonSettingData = _commonSettingsRepository.GetCommonSetting();
                        AzureDataExporerHelper.kustoUri = commonSettingData.KustoUri;
                        AzureDataExporerHelper.clientId = commonSettingData.ClientId;
                        AzureDataExporerHelper.clientSecret = commonSettingData.ClientSecret;
                        AzureDataExporerHelper.tenantId = commonSettingData.TenantId;
                        AzureDataExporerHelper.databaseName = commonSettingData.DatabaseName;
                        foreach (var item in deviceNames)
                        {
                            status = AzureDataExporerHelper.DeleteTable(item.DeviceName.Replace(" ", "_"));
                            if (status)
                            {
                                int[] ids = new int[] { item.Id };
                                _IoTDevicePropertyRepository.DeleteDeviceProperties(ids);
                            }
                        }
                        return RedirectToAction("Index", "IoTDeviceProperty", new { msg = "deleted" });

                    }
                    return RedirectToAction("Index", "IoTDeviceProperty", new { msg = "inuse" });
                }
                else
                {
                    return RedirectToAction("Index", "IoTDeviceProperty", new { msg = "noselect" });
                }
            }
            catch (Exception _exception)
            {
                if (_exception.InnerException.Message.Contains(GlobalCode.foreignKeyReference) || ((_exception.InnerException).InnerException).Message.Contains(GlobalCode.foreignKeyReference))
                {
                    return RedirectToAction("Index", "IoTDeviceProperty", new { msg = "inuse" });
                }
                throw _exception;
            }
        }
        #endregion

        #region Detail
        [HttpGet]
        public ActionResult Detail(int id)
        {
            if (id > 0)
            {
                IoTDevicePropertyViewModel ioTDevicePropertyViewModel = new IoTDevicePropertyViewModel();
                var ioTDevicePropertyModel = _IoTDevicePropertyRepository.GetDevicesPropertyDetailById(id);
                if (ioTDevicePropertyModel == null)
                {
                    return RedirectToAction("Index", "IoT Device Property", new { msg = "drop" });
                }
                ioTDevicePropertyViewModel.ioTDeviceProperties = ioTDevicePropertyModel;
                ioTDevicePropertyViewModel.DeviceName = ioTDevicePropertyModel.Select(e => e.DeviceName).FirstOrDefault();
                ioTDevicePropertyViewModel.Active = ioTDevicePropertyModel.Select(e => e.Active).FirstOrDefault();
                return View(ioTDevicePropertyViewModel);
            }
            else
            {
                return RedirectToAction("Index", "IoT Device Property", new { msg = "error" });
            }
        }
        #endregion

        #region Bind All Dropdown

        private IoTDevicePropertyViewModel BindDropDown(IoTDevicePropertyViewModel ioTDevicePropertyViewModel, bool isEditable)
        {
            // Device binding
            ioTDevicePropertyViewModel.SelectedDevice = new SelectList(_IoTDeviceRepository.BindIoTDeviceDropdownList(isEditable), "value", "name", ioTDevicePropertyViewModel.IoTDeviceId);


            // Data type binding
            if (ioTDevicePropertyViewModel.DataTypeId > 0)
            {
                ioTDevicePropertyViewModel.SelectDataType = new SelectList(_IoTDeviceRepository.GetDataTypeDropdownList(0), "value", "name", ioTDevicePropertyViewModel.DataTypeId);
            }
            else
            {
                ioTDevicePropertyViewModel.SelectDataType = new SelectList(_IoTDeviceRepository.GetDataTypeDropdownList(0), "value", "name");
            }

            return ioTDevicePropertyViewModel;
        }
        #endregion
    }
}