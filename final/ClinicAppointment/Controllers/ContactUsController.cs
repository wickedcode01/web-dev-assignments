using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointment.Controllers
{
    public class ContactUsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
