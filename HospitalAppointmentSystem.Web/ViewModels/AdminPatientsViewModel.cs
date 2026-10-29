using HospitalAppointmentSystem.Web.Models;

namespace HospitalAppointmentSystem.Web.ViewModels;

public class AdminPatientsViewModel
{
    public List<Patient> Patients { get; set; } = new();
    public Dictionary<int, int> AppointmentCounts { get; set; } = new();
}