using HospitalAppointmentSystem.Web.Models;

namespace HospitalAppointmentSystem.Web.ViewModels;

public class SecretaryBookViewModel
{
    public string? NationalId { get; set; }
    public Patient? Patient { get; set; }
    public bool PatientNotFound { get; set; }

    public List<Doctor> Doctors { get; set; } = new();
    public int? DoctorId { get; set; }
    public HealthcareProfessional? SelectedDoctor { get; set; }

    public List<MedicalService> Services { get; set; } = new();
    public int? ServiceId { get; set; }
    public MedicalService? SelectedService { get; set; }

    public DateTime? Date { get; set; }
    public List<DateTime> AvailableTimes { get; set; } = new();
}