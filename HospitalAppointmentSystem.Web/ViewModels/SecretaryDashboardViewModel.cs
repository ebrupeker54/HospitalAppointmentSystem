using HospitalAppointmentSystem.Web.Models;

namespace HospitalAppointmentSystem.Web.ViewModels;

public class SecretaryDashboardViewModel
{
    public List<Doctor> Doctors { get; set; } = new();
    public int? SelectedDoctorId { get; set; }
    public List<Appointment> Appointments { get; set; } = new();
    public List<AvailabilitySlot> Slots { get; set; } = new();

    public Doctor? SelectedDoctor => Doctors.FirstOrDefault(d => d.Id == SelectedDoctorId);
}