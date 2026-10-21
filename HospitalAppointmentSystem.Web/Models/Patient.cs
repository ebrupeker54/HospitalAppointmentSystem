namespace HospitalAppointmentSystem.Web.Models;

public class Patient : User
{
    public string NationalId { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }

    public Patient()
    {
        Role = UserRole.Patient;
    }

    public override string DashboardController => "Patient";

    public override string GetRoleDescription()
    {
        return "Hasta - randevu alabilir ve randevularını yönetebilir";
    }
}