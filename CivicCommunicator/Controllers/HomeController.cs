using Microsoft.AspNetCore.Mvc;

namespace CivicCommunicator.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}