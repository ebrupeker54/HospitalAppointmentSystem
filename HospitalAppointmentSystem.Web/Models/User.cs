using Microsoft.AspNetCore.Identity;

namespace HospitalAppointmentSystem.Web.Models;

public abstract class User
{
    private static readonly PasswordHasher<User> Hasher = new();

    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    public UserRole Role { get; protected set; }

    public string FullName => $"{FirstName} {LastName}";

    public void SetPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters.");

        PasswordHash = Hasher.HashPassword(this, password);
    }

    public bool VerifyPassword(string password)
    {
        if (string.IsNullOrEmpty(PasswordHash))
            return false;

        return Hasher.VerifyHashedPassword(this, PasswordHash, password) != PasswordVerificationResult.Failed;
    }

    public abstract string GetRoleDescription();
}