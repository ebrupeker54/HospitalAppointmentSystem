namespace HospitalAppointmentSystem.Web.Models;

public class LabTechnician : HealthStaff, IResultEntry
{
    public override string Title => "Laborant";

    protected override ServiceCategory AllowedCategory => ServiceCategory.Laboratory;

    public string ResultTitle => "Tahlil Sonucu";

    public TestResult WriteResult(Appointment appointment, string findings) =>
        new(appointment, this, ResultTitle, findings);

    public override string GetRoleDescription()
    {
        return "Laborant - laboratuvar hizmetlerini verebilir ve tahlil sonucu girebilir";
    }
}