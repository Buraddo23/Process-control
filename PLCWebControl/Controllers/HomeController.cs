using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PLCWebControl.Models;
using PLCWebControl.Services;

namespace PLCWebControl.Controllers
{
    public class HomeController : Controller
    {
        private ITcpService _service;

        public HomeController(ITcpService service)
        {
            _service = service;
        }

        //This is the default action
        public IActionResult Index()
        {
            var lastData = _service.GetLastData();
            ViewData["Buttons"] = lastData.Buttons;
            ViewData["Sensors"] = lastData.Sensors;
            ViewData["WaterLevel"] = lastData.WaterLevel;
            ViewData["Inflow"] = lastData.Inflow;
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Send(CommandDataModel data)
        {
            if (ModelState.IsValid)
            {
                _service.SendData(data);
                return RedirectToAction("Index");
            }
            else
            {
                var lastData = _service.GetLastData();
                return View("Index", lastData);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
