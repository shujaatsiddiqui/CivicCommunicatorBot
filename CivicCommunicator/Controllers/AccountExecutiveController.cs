using Microsoft.AspNetCore.Mvc;

namespace CivicCommunicator.Controllers
{
    public class AccountExecutiveController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}