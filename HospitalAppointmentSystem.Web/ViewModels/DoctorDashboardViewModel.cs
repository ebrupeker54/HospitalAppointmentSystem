using HospitalAppointmentSystem.Web.Models;

namespace HospitalAppointmentSystem.Web.ViewModels;

public class DoctorDashboardViewModel
{
    public List<Appointment> Appointments { get; set; } = new();
    public List<AvailabilitySlot> UpcomingSlots { get; set; } = new();
}