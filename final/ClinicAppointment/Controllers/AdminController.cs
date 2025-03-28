using Microsoft.AspNetCore.Mvc;
using ClinicAppointment.Data;
using ClinicAppointment.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

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

        // GET: Admin/Edit/5
        public IActionResult Edit(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, [Bind("Id, PatientEmail, Phone, AppointmentDate, Status")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var existingAppointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
                if (existingAppointment == null)
                {
                    return NotFound();
                }

                existingAppointment.PatientEmail = appointment.PatientEmail;
                existingAppointment.Phone = appointment.Phone;
                existingAppointment.AppointmentDate = appointment.AppointmentDate;
                existingAppointment.Status = appointment.Status;

                _context.SaveChanges();
                return RedirectToAction(nameof(Appointments));
            }
            return View(appointment);
        }
    }
}
