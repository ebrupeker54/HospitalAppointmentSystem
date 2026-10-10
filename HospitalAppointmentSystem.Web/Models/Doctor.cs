namespace HospitalAppointmentSystem.Web.Models;

public class Doctor : User
{
    public string Specialty { get; set; } = string.Empty;

    public Doctor()
    {
        Role = UserRole.Doctor;
    }

    public override string GetRoleDescription()
    {
        return $"Doctor ({Specialty}) - can manage availability and approve appointments";
    }
}