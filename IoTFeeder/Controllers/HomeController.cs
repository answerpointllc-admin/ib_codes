using Microsoft.AspNetCore.Mvc;
using IoTFeeder.Common.Common;
using IoTFeeder.Common.Helpers;
using IoTFeeder.Common.Models;
using System.Diagnostics;

namespace IoTFeeder.Admin.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]

        public IActionResult Home()
        {
            // Get Dashboard Data       --  Start

            // Get Dashboard Data       --  End
            return View();
        }

        #region Logout
        /// <summary>
        /// Logout Method
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }
        #endregion

        public IActionResult EditorToken()
        {
            return Json(Encryption.DecryptWithUrlDecode(GlobalCode.RandomString(15)));
        }
    }
}