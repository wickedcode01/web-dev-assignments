using Microsoft.AspNetCore.Mvc;
using ClinicAppointment.Data;
using Microsoft.AspNetCore.Authorization;

namespace ClinicAppointment.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Appointments()
        {
            var appointments = _context.Appointments.OrderBy(a => a.AppointmentDate).ToList();
            return View(appointments);
        }
    }
}
