using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using ClinicAppointment.Data;
using ClinicAppointment.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointment.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

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
        public async Task<IActionResult> Book(Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                // Log validation errors
                foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    System.Diagnostics.Debug.WriteLine($"Validation Error: {modelError.ErrorMessage}");
                }
            }

            if (ModelState.IsValid)
            {
                // Set the user ID for the appointment
                appointment.UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                appointment.Status = AppointmentStatus.Pending;
                appointment.ConfirmationNumber = GenerateConfirmationNumber();

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                // Add success message
                TempData["SuccessMessage"] = "Appointment booked successfully! Your confirmation number is: " + appointment.ConfirmationNumber;
                
                // Redirect to home page
                return RedirectToAction("Index", "Home");
            }

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

        private string GenerateConfirmationNumber()
        {
            // Generate a random confirmation number
            return "CONF-" + DateTime.Now.Ticks.ToString().Substring(0, 8);
        }

        public async Task<IActionResult> Confirmation(string id)
        {
            // Get the appointment from database
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.ConfirmationNumber == id);

            if (appointment == null)
            {
                // If appointment not found, show error message and redirect to home
                TempData["ErrorMessage"] = "Appointment not found.";
                return RedirectToAction("Index", "Home");
            }

            // Update appointment status to confirmed
            appointment.Status = AppointmentStatus.Confirmed;
            await _context.SaveChangesAsync();

            // Show success message
            TempData["SuccessMessage"] = "Appointment confirmed successfully!";
            
            return View(appointment);
        }
    }
} 