namespace HospitalAppointmentSystem.Web.Models;

public class Institution
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public InstitutionType Type { get; set; }

    public List<Branch> Branches { get; private set; } = new();
    public List<MedicalService> Services { get; private set; } = new();

    public bool IsMultiBranch => Branches.Count > 1;

    public void AddBranch(Branch branch)
    {
        if (Branches.Any(b => string.Equals(b.Name, branch.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"A branch named '{branch.Name}' already exists.");

        branch.Institution = this;
        Branches.Add(branch);
    }

    public void AddService(MedicalService service)
    {
        if (Services.Any(s => string.Equals(s.Name, service.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"A service named '{service.Name}' already exists.");

        service.Institution = this;
        Services.Add(service);
    }
}