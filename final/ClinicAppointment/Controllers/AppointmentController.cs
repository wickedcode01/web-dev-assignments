using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicAppointment.Data;
using ClinicAppointment.Models;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace ClinicAppointment.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IClock _clock;

        public AppointmentController(ApplicationDbContext context, IClock clock)
        {
            _context = context;
            _clock = clock;
        }

        [HttpGet]
        public IActionResult Book(string? Type, string appointmentDate)
        {
            var timeZone = DateTimeZoneProviders.Tzdb["America/Toronto"];
            var now = _clock.GetCurrentInstant().InZone(timeZone);

            ZonedDateTime dateToUse;
            if (string.IsNullOrEmpty(appointmentDate))
            {
                dateToUse = now;
            }
            else
            {
                if (!DateTime.TryParse(appointmentDate, out DateTime parsedDate))
                {
                    ModelState.AddModelError("AppointmentDate", "Invalid date format.");
                    return View();
                }

                var localDateTime = LocalDateTime.FromDateTime(parsedDate);
                dateToUse = timeZone.AtLeniently(localDateTime);
            }
            var appointmentType = Type ?? "general-checkup";
            var availableSlots = GenerateAvailableSlots(dateToUse, out ZonedDateTime adjustedDate, appointmentType);

            ViewBag.AvailableTimeSlots = availableSlots;
            ViewBag.AppointmentType = appointmentType;
            ViewBag.AdjustedDate = adjustedDate;
            ViewBag.AppointmentDate = adjustedDate.ToString("yyyy-MM-dd", null); // Auto-updated date

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Book(Appointment appointment)
        {
            //if (appointment.AppointmentDate == default)
            //{
            //    ModelState.AddModelError("AppointmentDate", "The selected date is required.");
            //    return View(appointment);
            //}

            var timeZone = DateTimeZoneProviders.Tzdb["America/Toronto"];
            var now = _clock.GetCurrentInstant().InZone(timeZone);

            // Ensure the appointment date is not in the past
            if (appointment.AppointmentDate < now.ToDateTimeUnspecified().Date)
            {
                ModelState.AddModelError("AppointmentDate", "The date cannot be in the past.");
                return View(appointment);
            }

            var localDateTime = LocalDateTime.FromDateTime(appointment.AppointmentDate);
            var dateToUse = timeZone.AtLeniently(localDateTime);

            // Get available slots for the selected date
            var availableSlots = GenerateAvailableSlots(dateToUse, out ZonedDateTime adjustedDate, appointment.AppointmentType);

            // Ensure the selected time slot is still available
            if (!availableSlots.Contains(appointment.TimeSlot))
            {
                ModelState.AddModelError("TimeSlot", "The selected time slot is no longer available. Please choose another.");
                ViewBag.AvailableTimeSlots = availableSlots;
                ViewBag.AppointmentDate = adjustedDate.ToString("yyyy-MM-dd", null);
                return View(appointment);
            }

            // Assign User ID and generate Confirmation Number
            appointment.UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            appointment.ConfirmationNumber = GenerateConfirmationNumber();
            appointment.Status = AppointmentStatus.Pending;

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"{error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                ViewBag.AvailableTimeSlots = availableSlots;
                ViewBag.AppointmentDate = adjustedDate.ToString("yyyy-MM-dd", null);
                return View(appointment);
            }

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Appointment booked successfully! Your confirmation number is: " + appointment.ConfirmationNumber;
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult UpdateAvailableSlots(string appointmentDate, string appointmentType)
        {
            if (!DateTime.TryParse(appointmentDate, out DateTime parsedDate))
            {
                return BadRequest("Invalid date format.");
            }

            var timeZone = DateTimeZoneProviders.Tzdb["America/Toronto"];
            var localDateTime = LocalDateTime.FromDateTime(parsedDate);
            var dateToUse = timeZone.AtLeniently(localDateTime);

            var availableSlots = GenerateAvailableSlots(dateToUse, out _, appointmentType);

            return PartialView("_AvailableSlotsPartial", availableSlots);
        }

        public List<string> GenerateAvailableSlots(ZonedDateTime selectedDate, out ZonedDateTime adjustedDate, string appointmentType)
        {
            var availableSlots = new List<string>();
            int startHour = 9, endHour = 18; // Business hours (9 AM - 6 PM)
            var slotDuration = Period.FromMinutes(15); // Slot duration (15 minutes)

            ZonedDateTime now = _clock.GetCurrentInstant().InZone(selectedDate.Zone);
            adjustedDate = selectedDate;

            if (selectedDate.Date == now.Date)
            {
                int currentHour = now.Hour;
                int currentMinute = now.Minute;

                if (currentHour >= startHour)
                {
                    startHour = currentMinute > 0 ? currentHour + 1 : currentHour;
                }
            }

            while (true)
            {
                availableSlots.Clear();
                LocalDate selectedLocalDate = selectedDate.Date;

                for (var time = new LocalTime(startHour, 0); time.Hour < endHour; time = time.Plus(slotDuration))
                {
                    string slot = time.ToString("HH:mm", null);

                    bool isSlotTaken = _context.Appointments
                        .AsEnumerable() // Switch to client-side evaluation
                        .Any(a => a.TimeSlot == slot && a.AppointmentDate.Date == selectedLocalDate.ToDateTimeUnspecified().Date && a.AppointmentType == appointmentType);

                    if (!isSlotTaken)
                    {
                        Console.WriteLine(isSlotTaken.ToString(), "isslot taken");
                        availableSlots.Add(slot);
                    }
                }

                if (availableSlots.Any())
                {
                    return availableSlots;
                }

                adjustedDate = adjustedDate.Plus(Duration.FromDays(1));
                startHour = 9;
            }
        }

        private string GenerateConfirmationNumber()
        {
            return "CONF-" + DateTime.Now.Ticks.ToString().Substring(0, 8);
        }

        public async Task<IActionResult> Confirmation(string id)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.ConfirmationNumber == id);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Appointment not found.";
                return RedirectToAction("Index", "Home");
            }

            appointment.Status = AppointmentStatus.Confirmed;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Appointment confirmed successfully!";
            return View(appointment);
        }



    }
}
