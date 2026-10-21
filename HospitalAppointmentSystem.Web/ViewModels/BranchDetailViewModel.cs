using HospitalAppointmentSystem.Web.Models;

namespace HospitalAppointmentSystem.Web.ViewModels;

public class BranchDetailsViewModel
{
    public Branch Branch { get; set; } = null!;
    public List<DayScheduleInput> Schedule { get; set; } = new();
    public Dictionary<int, int> DoctorCounts { get; set; } = new();
}