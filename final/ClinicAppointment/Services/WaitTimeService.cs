using ClinicAppointment.Data;
using ClinicAppointment.Models;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace ClinicAppointment.Services
{
    public class WaitTimeService : IWaitTimeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IClock _clock;

        public WaitTimeService(ApplicationDbContext context, IClock clock)
        {
            _context = context;
            _clock = clock;
        }

        public async Task<int> GetCurrentWaitTimeAsync()
        {
            var timeZone = DateTimeZoneProviders.Tzdb["America/Toronto"];
            var now = _clock.GetCurrentInstant().InZone(timeZone);
            var today = now.Date;

            // Get all pending appointments for today
            var todayAppointments = await _context.Appointments
                .Where(a => a.AppointmentDate.Date == today.ToDateTimeUnspecified().Date 
                    && a.Status == AppointmentStatus.Pending)
                .OrderBy(a => a.TimeSlot)
                .ToListAsync();

            if (!todayAppointments.Any())
            {
                return 0; // No pending appointments today, no wait time
            }

            // Calculate average wait time based on number of pending appointments
            // Assuming each appointment takes 30 minutes
            const int appointmentDuration = 30;
            var totalAppointments = todayAppointments.Count;
            
            // Base wait time calculation: (number of pending appointments * appointment duration) / 2
            // This gives us an average wait time based on the number of pending appointments
            var baseWaitTime = (totalAppointments * appointmentDuration) / 2;

            // Add some buffer time for unexpected delays
            var bufferTime = 5*totalAppointments;
            return baseWaitTime + bufferTime;
        }
    }
} 