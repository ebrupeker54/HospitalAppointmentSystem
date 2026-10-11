namespace HospitalAppointmentSystem.Web.Models;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int BranchId { get; set; }
    public Branch? Branch { get; set; }
}