using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointment.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
