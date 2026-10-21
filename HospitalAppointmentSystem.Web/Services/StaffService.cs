using HospitalAppointmentSystem.Web.Data;
using HospitalAppointmentSystem.Web.Models;
using HospitalAppointmentSystem.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Services;

public class StaffService
{
    private readonly AppDbContext _context;

    public StaffService(AppDbContext context)
    {
        _context = context;
    }

    public void CreateDoctor(DoctorFormViewModel form)
    {
        var email = NormalizeEmail(form.Email);
        if (_context.Users.Any(u => u.Email == email))
            throw new InvalidOperationException("An account with this email already exists.");

        var doctor = new Doctor
        {
            FirstName = form.FirstName.Trim(),
            LastName = form.LastName.Trim(),
            Email = email,
            PhoneNumber = form.PhoneNumber.Trim(),
            Specialty = form.Specialty.Trim()
        };
        doctor.SetPassword(form.Password ?? string.Empty);

        ApplyAssignments(doctor, form);

        _context.Doctors.Add(doctor);
        _context.SaveChanges();
    }

    public void UpdateDoctor(DoctorFormViewModel form)
    {
        var doctor = _context.Doctors
            .Include(d => d.Services)
            .FirstOrDefault(d => d.Id == form.Id)
            ?? throw new KeyNotFoundException("Doctor not found.");

        var email = NormalizeEmail(form.Email);
        if (_context.Users.Any(u => u.Id != doctor.Id && u.Email == email))
            throw new InvalidOperationException("An account with this email already exists.");

        doctor.FirstName = form.FirstName.Trim();
        doctor.LastName = form.LastName.Trim();
        doctor.Email = email;
        doctor.PhoneNumber = form.PhoneNumber.Trim();
        doctor.Specialty = form.Specialty.Trim();

        ApplyAssignments(doctor, form);
        _context.SaveChanges();
    }

    private void ApplyAssignments(Doctor doctor, DoctorFormViewModel form)
    {
        var branch = _context.Branches
            .Include(b => b.Institution)
            .Include(b => b.Departments)
            .FirstOrDefault(b => b.Id == form.BranchId)
            ?? throw new ArgumentException("Please select a branch.");

        var department = form.DepartmentId == null
            ? null
            : branch.Departments.FirstOrDefault(d => d.Id == form.DepartmentId)
              ?? throw new InvalidOperationException("The selected department does not belong to the selected branch.");

        var requestedIds = form.ServiceIds.Distinct().ToList();
        var services = _context.MedicalServices
            .Where(s => requestedIds.Contains(s.Id) && s.InstitutionId == branch.InstitutionId)
            .ToList();

        if (services.Count != requestedIds.Count)
            throw new InvalidOperationException("Doctors can only offer services of their branch's institution.");

        doctor.AssignTo(branch, department);
        doctor.SetServices(services);
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}