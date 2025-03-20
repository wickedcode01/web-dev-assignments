using Microsoft.AspNetCore.Mvc;
using System;
using ClinicAppointment.Models;

namespace ClinicAppointment.Controllers
{
    public class AppointmentController : Controller
    {
        [HttpGet]
        public IActionResult Book(string id = null)
        {
            // In a real application, you would fetch available time slots from a service
            var availableTimeSlots = new[]
            {
                new { Time = "9:00", Value = "09:00" },
                new { Time = "10:15", Value = "10:15" },
                new { Time = "11:30", Value = "11:30" }
            };

            ViewBag.AvailableTimeSlots = availableTimeSlots;
            ViewBag.AppointmentType = id ?? "general-checkup";
            
            return View(new Appointment());
        }

        [HttpPost]
        public IActionResult Book(Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                // Initialize available time slots when returning the view
                var availableTimeSlots = new[]
                {
                    new { Time = "9:00", Value = "09:00" },
                    new { Time = "10:15", Value = "10:15" },
                    new { Time = "11:30", Value = "11:30" }
                };
                ViewBag.AvailableTimeSlots = availableTimeSlots;
                return View(appointment);
            }

            // In a real application, you would save the appointment to a database
            appointment.ConfirmationNumber = "HC-" + DateTime.Now.ToString("yyyyMMdd-HHmm");
            appointment.Status = AppointmentStatus.Confirmed;

            return RedirectToAction("Confirmation", new { id = appointment.ConfirmationNumber });
        }

        public IActionResult Confirmation(string id)
        {
            // In a real application, you would fetch the appointment details from a database
            var appointment = new Appointment
            {
                ConfirmationNumber = id,
                AppointmentDate = DateTime.Now.AddDays(7),
                AppointmentType = "General Consultation",
                Status = AppointmentStatus.Confirmed
            };

            return View(appointment);
        }
    }
} 