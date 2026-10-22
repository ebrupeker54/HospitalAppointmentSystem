namespace HospitalAppointmentSystem.Web.Models;

public class RadiologyTechnician : HealthStaff
{
    public override string Title => "Radyoloji Teknikeri";

    protected override ServiceCategory AllowedCategory => ServiceCategory.Radiology;

    public override string GetRoleDescription()
    {
        return "Radyoloji Teknikeri - röntgen, ultrason gibi görüntüleme hizmetlerini verebilir";
    }
}