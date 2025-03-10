using Microsoft.AspNetCore.Mvc;

namespace RAILab2.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult PageNotFound()
        {
            return View();
        }
    }
}
