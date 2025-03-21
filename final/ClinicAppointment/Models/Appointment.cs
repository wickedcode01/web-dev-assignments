using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicAppointment.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public string AppointmentType { get; set; }

        [Required]
        public string TimeSlot { get; set; }

        [Required]
        public string PatientName { get; set; }



        [Required]
        [EmailAddress]
        public string PatientEmail { get; set; }

        public string? Description { get; set; }
        
        public string? UserId { get; set; }
        
        public string? ConfirmationNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    }

    public enum AppointmentStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }
} 