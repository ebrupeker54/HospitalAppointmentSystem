namespace HospitalAppointmentSystem.Web.Models;

public class Nurse : HealthStaff
{
    public override string Title => "Hemşire";

    protected override ServiceCategory AllowedCategory => ServiceCategory.Nursing;

    public override string GetRoleDescription()
    {
        return "Hemşire - enjeksiyon, pansuman gibi hemşirelik hizmetlerini verebilir";
    }
}