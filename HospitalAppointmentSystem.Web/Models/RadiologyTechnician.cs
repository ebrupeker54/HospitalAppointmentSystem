namespace HospitalAppointmentSystem.Web.Models;

public class RadiologyTechnician : HealthStaff, IResultEntry
{
    public override string Title => "Radyoloji Teknikeri";

    protected override ServiceCategory AllowedCategory => ServiceCategory.Radiology;

    public string ResultTitle => "Görüntüleme Raporu";

    public TestResult WriteResult(Appointment appointment, string findings) =>
        new(appointment, this, ResultTitle, findings);

    public override string GetRoleDescription()
    {
        return "Radyoloji Teknikeri - görüntüleme hizmetlerini verebilir ve rapor yazabilir";
    }
}