namespace HospitalAppointmentSystem.Web.Models;

public class Doctor : HealthcareProfessional
{
    public string Specialty { get; set; } = string.Empty;
    public List<Secretary> Secretaries { get; private set; } = new();
    public Doctor()
    {
        Role = UserRole.Doctor;
    }

    public override string Title => "Dr.";

    public override bool CanOffer(MedicalService service) => true;

    public override string GetRoleDescription()
    {
        return $"Doktor ({Specialty}) - muayene dahil tüm hizmetleri verebilir ve randevularını yönetebilir";
    }
}