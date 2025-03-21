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
        public IActionResult Book(string? Type )
        {
            // In a real application, you would fetch available time slots from a service
            var availableTimeSlots = new[]
            {
                new { Time = "9:00", Value = "09:00" },
                new { Time = "10:00", Value = "10:00" },
                new { Time = "11:00", Value = "11:00" },
                new { Time = "12:00", Value = "12:00" },
                new { Time = "13:00", Value = "13:00" },
                new { Time = "14:00", Value = "14:00" },
            };

            ViewBag.AvailableTimeSlots = availableTimeSlots;
            ViewBag.AppointmentType = Type ?? "general-checkup";
            System.Diagnostics.Debug.WriteLine($"Type: {Type}");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Book(Appointment appointment)
        {
            
            appointment.UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            appointment.ConfirmationNumber = GenerateConfirmationNumber();

            // // Clear validation errors for system-set fields
            // ModelState.Remove("UserId");
            // ModelState.Remove("ConfirmationNumber");

            if (!ModelState.IsValid)
            {
                // Log detailed validation errors
                foreach (var modelStateEntry in ModelState)
                {
                    var propertyName = modelStateEntry.Key;
                    foreach (var error in modelStateEntry.Value.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Property {propertyName} failed validation: {error.ErrorMessage}");
                        System.Diagnostics.Debug.WriteLine($"Error State: {modelStateEntry.Value.ValidationState}");
                        System.Diagnostics.Debug.WriteLine($"Raw Value: {modelStateEntry.Value.RawValue}");
                        System.Diagnostics.Debug.WriteLine($"Attempted Value: {modelStateEntry.Value.AttemptedValue}");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                appointment.Status = AppointmentStatus.Pending;
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
                new { Time = "10:00", Value = "10:00" },
                new { Time = "11:00", Value = "11:00" },
                new { Time = "12:00", Value = "12:00" },
                new { Time = "13:00", Value = "13:00" },
                new { Time = "14:00", Value = "14:00" },
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