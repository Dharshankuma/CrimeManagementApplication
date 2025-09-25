using Microsoft.AspNetCore.Mvc;

namespace CrimeManagement.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
