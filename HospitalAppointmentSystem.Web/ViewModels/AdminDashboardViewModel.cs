using HospitalAppointmentSystem.Web.Models;

namespace HospitalAppointmentSystem.Web.ViewModels;

public class AdminDashboardViewModel
{
    public int InstitutionCount { get; set; }
    public int BranchCount { get; set; }
    public int DoctorCount { get; set; }
    public int PatientCount { get; set; }
    public int PendingAppointmentCount { get; set; }
    public List<Doctor> Doctors { get; set; } = new();
}