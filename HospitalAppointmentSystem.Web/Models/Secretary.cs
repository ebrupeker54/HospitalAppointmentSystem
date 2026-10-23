namespace HospitalAppointmentSystem.Web.Models;

public class Secretary : Staff, IAppointmentManager
{
    public List<Doctor> Doctors { get; private set; } = new();

    public Secretary()
    {
        Role = UserRole.Secretary;
    }

    public override string DashboardController => "Secretary";
    public override string Title => "Sekreter";

    public void SetDoctors(IEnumerable<Doctor> doctors)
    {
        foreach (var existing in Doctors.ToList())
            UnassignDoctor(existing);

        foreach (var doctor in doctors)
            AssignDoctor(doctor);
    }
    public bool CanManage(int professionalId) => Doctors.Any(d => d.Id == professionalId);

    public void AssignDoctor(Doctor doctor)
    {
        if (Branch != null && doctor.Branch != null && doctor.Branch != Branch)
            throw new InvalidOperationException("Sekreter yalnızca kendi şubesindeki doktorlara atanabilir.");

        if (!Doctors.Contains(doctor))
            Doctors.Add(doctor);
    }

    public void UnassignDoctor(Doctor doctor)
    {
        Doctors.Remove(doctor);
    }

    public override string GetRoleDescription()
    {
        return $"Sekreter - {Doctors.Count} doktorun randevularını ve uygunluklarını yönetebilir";
    }
}