using System;
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

        public ActionResult SendButton(string button)
        {
            if (!String.IsNullOrWhiteSpace(button))
            {
                var command = new CommandDataModel();
                switch (button)
                {
                    case "Off":
                        command.Buttons = 128;
                        command.DesiredInflow = 0;
                        _service.SendData(command);
                        break;
                    case "On":
                        command.Buttons = 64;
                        command.DesiredInflow = 128;
                        _service.SendData(command);
                        break;
                    case "P1":
                        command.Buttons = 16;
                        command.DesiredInflow = 128;
                        _service.SendData(command);
                        break;
                    case "P2":
                        command.Buttons = 8;
                        command.DesiredInflow = 128;
                        _service.SendData(command);
                        break;
                    case "RST":
                        command.Buttons = 4;
                        command.DesiredInflow = 128;
                        _service.SendData(command);
                        break;
                    default:
                        
                        break;
                }
            }
            return View("Index");
        }

        [HttpGet]
        public JsonResult Stare() {         
            return Json(_service.GetLastData());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
