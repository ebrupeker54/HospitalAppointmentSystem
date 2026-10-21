using HospitalAppointmentSystem.Web.Data;
using HospitalAppointmentSystem.Web.Models;

namespace HospitalAppointmentSystem.Web.Services;

public class AuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public User? ValidateCredentials(string email, string password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = _context.Users.FirstOrDefault(u => u.Email == normalizedEmail);

        return user != null && user.VerifyPassword(password) ? user : null;
    }

    public Patient RegisterPatient(string firstName, string lastName, string email, string phoneNumber,
        string nationalId, DateTime dateOfBirth, string password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        if (_context.Users.Any(u => u.Email == normalizedEmail))
            throw new InvalidOperationException("Bu e-posta adresiyle kayıtlı bir hesap zaten var.");

        if (_context.Patients.Any(p => p.NationalId == nationalId))
            throw new InvalidOperationException("Bu T.C. kimlik numarasıyla kayıtlı bir hasta zaten var.");

        var patient = new Patient
        {
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = normalizedEmail,
            PhoneNumber = phoneNumber.Trim(),
            NationalId = nationalId,
            DateOfBirth = dateOfBirth
        };
        patient.SetPassword(password);

        _context.Patients.Add(patient);
        _context.SaveChanges();
        return patient;
    }
}