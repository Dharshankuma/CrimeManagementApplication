using Microsoft.AspNetCore.Mvc;

namespace CrimeManagement.Controllers.Masters
{
    public class StateMasterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
