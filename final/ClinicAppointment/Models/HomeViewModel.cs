using System;
using System.Collections.Generic;

namespace ClinicAppointment.Models
{
    public class HomeViewModel
    {
        public int CurrentWaitTime { get; set; }
        public required List<AppointmentType> AvailableAppointmentTypes { get; set; }
    }

    public class AppointmentType
    {
        public required string Type { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
} 