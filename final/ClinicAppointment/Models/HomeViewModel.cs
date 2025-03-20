using System;
using System.Collections.Generic;

namespace ClinicAppointment.Models
{
    public class HomeViewModel
    {
        public int CurrentWaitTime { get; set; }
        public List<AppointmentType> AvailableAppointmentTypes { get; set; }
    }

    public class AppointmentType
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
} 