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
using IoTFeeder.Helper;

namespace IoTFeeder.Admin.Controllers
{
    public class IoTDeviceController : Controller
    {
        #region Members Declaration
        private readonly IIoTDevice _IoTDeviceRepository;
        private readonly IIoTDeviceProperty _IoTDevicePropertyRepository;
        private readonly ICommonSettings _commonSettingsRepository;
        private static int _TotalCount = 0;

        public IoTDeviceController(IIoTDevice ioTDeviceRepository, IIoTDeviceProperty ioTDeviceProperty, ICommonSettings commonSettingsRepository)
        {
            this._IoTDeviceRepository = ioTDeviceRepository;
            this._IoTDevicePropertyRepository = ioTDeviceProperty;
            this._commonSettingsRepository = commonSettingsRepository;
        }
        #endregion

        #region Index
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.ModuleName = "IoTDevice";
            return View();
        }
        #endregion

        #region Ajax Binding
        public JsonResult _AjexBinding([DataSourceRequest] DataSourceRequest command, string searchValue)
        {
            var result = new DataSourceResult()
            {
                Data = GetIoTDeviceGridData(command, searchValue),
                Total = _TotalCount
            };
            return Json(result);
        }

        public IEnumerable GetIoTDeviceGridData([DataSourceRequest] DataSourceRequest command, string searchValue)
        {
            var result = _IoTDeviceRepository.GetDeviceList(searchValue);

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
            IoTDeviceViewModel ioTDeviceViewModel = new IoTDeviceViewModel();
            ioTDeviceViewModel.IsActive = true;
            return View(ioTDeviceViewModel);
        }

        [HttpPost]
        public ActionResult Create(IoTDeviceViewModel ioTDeviceViewModel)
        {
            ModelState.Remove("Active");
            ModelState.Remove("StrProperty");
            ModelState.Remove("Frequency");
            ModelState.Remove("FrequencyType");
            ModelState.Remove("FrequencyTypeText");
            ModelState.Remove("MinValue");
            ModelState.Remove("MaxValue");
            ModelState.Remove("Fixvalue");
            ModelState.Remove("ioTDeviceProperties");
            if (ModelState.IsValid)
            {
                _IoTDeviceRepository.SaveChanges(ioTDeviceViewModel);
                return RedirectToAction("Index", "IoTDevice", new { msg = "added" });
            }
            else
            {
                return View(ioTDeviceViewModel);
            }
        }
        #endregion

        #region Edit 
        [HttpGet]
        public ActionResult Edit(int id)
        {
            IoTDeviceViewModel ioTDeviceViewModel = _IoTDeviceRepository.GetDeviceDetailById(id);
            if (ioTDeviceViewModel == null)
            {
                return RedirectToAction("Index", "IoTDevice", new { msg = "drop" });
            }
            return View(ioTDeviceViewModel);

        }

        [HttpPost]
        public ActionResult Edit(IoTDeviceViewModel ioTDeviceViewModel)
        {
            ModelState.Remove("Active");
            ModelState.Remove("StrProperty");
            ModelState.Remove("Frequency");
            ModelState.Remove("FrequencyType");
            ModelState.Remove("FrequencyTypeText");
            ModelState.Remove("MinValue");
            ModelState.Remove("MaxValue");
            ModelState.Remove("Fixvalue");
            ModelState.Remove("ioTDeviceProperties");
            if (ModelState.IsValid)
            {
                _IoTDeviceRepository.SaveChanges(ioTDeviceViewModel);
                return RedirectToAction("Index", "IoTDevice", new { msg = "updated" });
            }
            else
            {
                return View(ioTDeviceViewModel);
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
                                _IoTDeviceRepository.DeleteDevices(chkDelete);
                            }
                        }
                        return RedirectToAction("Index", "IoTDevice", new { msg = "deleted" });

                    }
                    return RedirectToAction("Index", "IoTDevice", new { msg = "inuse" });
                }
                else
                {
                    return RedirectToAction("Index", "IoTDevice", new { msg = "noselect" });
                }
            }
            catch (Exception _exception)
            {
                if (_exception.InnerException.Message.Contains(GlobalCode.foreignKeyReference) || ((_exception.InnerException).InnerException).Message.Contains(GlobalCode.foreignKeyReference))
                {
                    return RedirectToAction("Index", "IoTDevice", new { msg = "inuse" });
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
                var ioTDeviceViewModel = _IoTDeviceRepository.GetDeviceDetailById(id);
                if (ioTDeviceViewModel == null)
                {
                    return RedirectToAction("Index", "IoTDevice", new { msg = "drop" });
                }
                return View(ioTDeviceViewModel);
            }
            else
            {
                return RedirectToAction("Index", "IoTDevice", new { msg = "error" });
            }
        }
        #endregion

    }
}