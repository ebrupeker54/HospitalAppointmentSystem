namespace HospitalAppointmentSystem.Web.Models;

public class Institution
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public InstitutionType Type { get; set; }

    public List<Branch> Branches { get; private set; } = new();

    public bool IsMultiBranch => Branches.Count > 1;

    public void AddBranch(Branch branch)
    {
        if (Branches.Any(b => string.Equals(b.Name, branch.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"A branch named '{branch.Name}' already exists.");

        branch.Institution = this;
        Branches.Add(branch);
    }
}