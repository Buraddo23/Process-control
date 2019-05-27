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
            return View();
        }

        [HttpGet]
        public JsonResult Stare() {         
            return Json(_service.GetLastData());
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
