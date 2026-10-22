namespace HospitalAppointmentSystem.Web.Models;

public class LabTechnician : HealthStaff
{
    public override string Title => "Laborant";

    protected override ServiceCategory AllowedCategory => ServiceCategory.Laboratory;

    public override string GetRoleDescription()
    {
        return "Laborant - kan tahlili gibi laboratuvar hizmetlerini verebilir";
    }
}