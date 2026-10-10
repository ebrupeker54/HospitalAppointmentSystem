namespace HospitalAppointmentSystem.Web.Models;

public abstract class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public UserRole Role { get; protected set; }

    public string FullName => $"{FirstName} {LastName}";

    public abstract string GetRoleDescription();
}