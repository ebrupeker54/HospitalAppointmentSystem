using HospitalAppointmentSystem.Web.Models;

namespace HospitalAppointmentSystem.Web.ViewModels;

public class BookAppointmentViewModel
{
    public int? BranchId { get; set; }
    public int? ServiceId { get; set; }
    public int? ProfessionalId { get; set; }
    public DateTime? Date { get; set; }

    public List<Branch> Branches { get; set; } = new();
    public List<MedicalService> Services { get; set; } = new();
    public List<HealthcareProfessional> Professionals { get; set; } = new();
    public List<DateTime> AvailableTimes { get; set; } = new();

    public MedicalService? SelectedService { get; set; }
    public HealthcareProfessional? SelectedProfessional { get; set; }
}