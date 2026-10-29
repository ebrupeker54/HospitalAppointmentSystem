using HospitalAppointmentSystem.Web.Models;

namespace HospitalAppointmentSystem.Web.ViewModels;

public class AdminAppointmentsViewModel
{
    public AppointmentStatus? Status { get; set; }
    public Patient? Patient { get; set; }
    public List<Appointment> Appointments { get; set; } = new();
}