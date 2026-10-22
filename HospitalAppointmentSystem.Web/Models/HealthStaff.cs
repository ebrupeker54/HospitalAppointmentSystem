namespace HospitalAppointmentSystem.Web.Models;

public abstract class HealthStaff : HealthcareProfessional
{
    protected HealthStaff()
    {
        Role = UserRole.HealthStaff;
    }

    protected abstract ServiceCategory AllowedCategory { get; }

    public sealed override bool CanOffer(MedicalService service) => service.Category == AllowedCategory;
}