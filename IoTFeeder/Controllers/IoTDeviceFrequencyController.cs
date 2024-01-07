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

namespace IoTFeeder.Admin.Controllers
{
    public class IoTDeviceFrequencyController : Controller
    {
        #region Members Declaration
        private readonly IIoTDevice _IoTDeviceRepository;
        private static int _TotalCount = 0;

        public IoTDeviceFrequencyController(IIoTDevice ioTDeviceRepository)
        {
            this._IoTDeviceRepository = ioTDeviceRepository;
        }
        #endregion

        #region Index
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.ModuleName = "IoT device frequency";
            return View();
        }
        #endregion

        #region Ajax Binding
        public JsonResult _AjexBinding([DataSourceRequest] DataSourceRequest command, string searchValue)
        {
            var result = new DataSourceResult()
            {
                Data = GetIoTDeviceFrequencyGridData(command, searchValue),
                Total = _TotalCount
            };
            return Json(result);
        }

        public IEnumerable GetIoTDeviceFrequencyGridData([DataSourceRequest] DataSourceRequest command, string searchValue)
        {
            var result = _IoTDeviceRepository.GetDeviceFrequencyList(searchValue);

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
            ioTDeviceViewModel = BindDropDown(ioTDeviceViewModel,false);
            ioTDeviceViewModel.IsActive = true;
            ioTDeviceViewModel.FrequencyType = false;
            return View(ioTDeviceViewModel);
        }

        [HttpPost]
        public ActionResult Create(IoTDeviceViewModel ioTDeviceViewModel)
        {
            ModelState.Remove("Active");
            ModelState.Remove("DeviceName");
            ModelState.Remove("Description");
            ModelState.Remove("StrProperty");
            ModelState.Remove("FrequencyTypeText");
            ModelState.Remove("ioTDeviceProperties");
            if (ModelState.IsValid)
            {
                _IoTDeviceRepository.SaveFrequence(ioTDeviceViewModel);
                return RedirectToAction("Index", "IoTDeviceFrequency", new { msg = "added" });
            }
            else
            {
                ioTDeviceViewModel = BindDropDown(ioTDeviceViewModel,false);
                return View(ioTDeviceViewModel);
            }
        }
        #endregion

        #region Edit 
        [HttpGet]
        public ActionResult Edit(int id)
        {
            IoTDeviceViewModel ioTDeviceViewModel = _IoTDeviceRepository.GetDeviceFrequencyDetailById(id);
            if (ioTDeviceViewModel == null)
            {
                return RedirectToAction("Index", "IoTDeviceFrequency", new { msg = "drop" });
            }
            ioTDeviceViewModel = BindDropDown(ioTDeviceViewModel,true);
            return View(ioTDeviceViewModel);

        }

        [HttpPost]
        public ActionResult Edit(IoTDeviceViewModel ioTDeviceViewModel)
        {
            ModelState.Remove("Active");
            ModelState.Remove("DeviceName");
            ModelState.Remove("Description");
            ModelState.Remove("StrProperty");
            ModelState.Remove("FrequencyTypeText");
            ModelState.Remove("ioTDeviceProperties");

            if (ModelState.IsValid)
            {
                _IoTDeviceRepository.SaveFrequence(ioTDeviceViewModel);
                return RedirectToAction("Index", "IoTDeviceFrequency", new { msg = "updated" });
            }
            else
            {
                ioTDeviceViewModel = BindDropDown(ioTDeviceViewModel,true);
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
                    _IoTDeviceRepository.DeleteDeviceFrequency(chkDelete);
                    return RedirectToAction("Index", "IoTDeviceFrequency", new { msg = "deleted" });
                }
                else
                {
                    return RedirectToAction("Index", "IoTDeviceFrequency", new { msg = "noselect" });
                }
            }
            catch (Exception _exception)
            {
                if (_exception.InnerException.Message.Contains(GlobalCode.foreignKeyReference) || ((_exception.InnerException).InnerException).Message.Contains(GlobalCode.foreignKeyReference))
                {
                    return RedirectToAction("Index", "IoTDeviceFrequency", new { msg = "inuse" });
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
                var ioTDeviceViewModel = _IoTDeviceRepository.GetDeviceFrequencyDetailById(id);
                if (ioTDeviceViewModel == null)
                {
                    return RedirectToAction("Index", "IoTDeviceFrequency", new { msg = "drop" });
                }
                return View(ioTDeviceViewModel);
            }
            else
            {
                return RedirectToAction("Index", "IoTDeviceFrequency", new { msg = "error" });
            }
        }
        #endregion

        #region Bind All Dropdown
        private IoTDeviceViewModel BindDropDown(IoTDeviceViewModel ioTDeviceViewModel,bool isEditable)
        {
            ioTDeviceViewModel.SelectedDevice = new SelectList(_IoTDeviceRepository.BindDeviceDropdownList(isEditable), "value", "name", ioTDeviceViewModel.Id);
            return ioTDeviceViewModel;
        }
        #endregion
    }
}