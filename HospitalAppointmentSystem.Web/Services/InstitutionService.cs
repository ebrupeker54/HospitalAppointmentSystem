using HospitalAppointmentSystem.Web.Data;
using HospitalAppointmentSystem.Web.Models;
using HospitalAppointmentSystem.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Services;

public class InstitutionService
{
    private readonly AppDbContext _context;

    public InstitutionService(AppDbContext context)
    {
        _context = context;
    }

    public void CreateInstitution(string name, InstitutionType type)
    {
        name = RequireText(name, "Institution name");

        if (_context.Institutions.Any(i => i.Name.ToLower() == name.ToLower()))
            throw new InvalidOperationException("An institution with this name already exists.");

        _context.Institutions.Add(new Institution { Name = name, Type = type });
        _context.SaveChanges();
    }

    public void UpdateInstitution(int id, string name, InstitutionType type)
    {
        name = RequireText(name, "Institution name");

        var institution = _context.Institutions.Find(id)
            ?? throw new KeyNotFoundException("Institution not found.");

        if (_context.Institutions.Any(i => i.Id != id && i.Name.ToLower() == name.ToLower()))
            throw new InvalidOperationException("An institution with this name already exists.");

        institution.Name = name;
        institution.Type = type;
        _context.SaveChanges();
    }

    public void AddBranch(int institutionId, string name, string? address, string? phoneNumber)
    {
        name = RequireText(name, "Branch name");

        var institution = _context.Institutions
            .Include(i => i.Branches)
            .FirstOrDefault(i => i.Id == institutionId)
            ?? throw new KeyNotFoundException("Institution not found.");

        institution.AddBranch(new Branch
        {
            Name = name,
            Address = address?.Trim() ?? string.Empty,
            PhoneNumber = phoneNumber?.Trim() ?? string.Empty
        });
        _context.SaveChanges();
    }

    public void UpdateBranch(int branchId, string name, string? address, string? phoneNumber)
    {
        name = RequireText(name, "Branch name");

        var branch = _context.Branches
            .Include(b => b.Institution).ThenInclude(i => i!.Branches)
            .FirstOrDefault(b => b.Id == branchId)
            ?? throw new KeyNotFoundException("Branch not found.");

        if (branch.Institution!.Branches.Any(b => b.Id != branchId
                && string.Equals(b.Name, name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"A branch named '{name}' already exists.");

        branch.Name = name;
        branch.Address = address?.Trim() ?? string.Empty;
        branch.PhoneNumber = phoneNumber?.Trim() ?? string.Empty;
        _context.SaveChanges();
    }

    public void AddDepartment(int branchId, string name)
    {
        name = RequireText(name, "Department name");

        var branch = LoadBranch(branchId);
        branch.AddDepartment(new Department { Name = name });
        _context.SaveChanges();
    }

    public void RemoveDepartment(int branchId, int departmentId)
    {
        var branch = LoadBranch(branchId);
        var department = branch.Departments.FirstOrDefault(d => d.Id == departmentId)
            ?? throw new KeyNotFoundException("Department not found.");

        if (_context.Doctors.Any(d => d.DepartmentId == departmentId))
            throw new InvalidOperationException("This department has doctors assigned and cannot be removed.");

        branch.RemoveDepartment(department);
        _context.SaveChanges();
    }

    public void UpdateSchedule(int branchId, IEnumerable<DayScheduleInput> days)
    {
        var branch = LoadBranch(branchId);

        foreach (var day in days)
        {
            if (!day.IsOpen)
            {
                branch.CloseOn(day.Day);
                continue;
            }

            if (day.OpenTime == null || day.CloseTime == null)
                throw new ArgumentException($"Please enter opening and closing times for {day.Day}.");

            branch.SetWorkingHours(new WorkingHours(day.Day,
                TimeOnly.FromTimeSpan(day.OpenTime.Value),
                TimeOnly.FromTimeSpan(day.CloseTime.Value)));
        }

        _context.SaveChanges();
    }

    public void AddService(int institutionId, string name, ServiceCategory category, int durationMinutes, decimal price)
    {
        name = RequireText(name, "Service name");

        var institution = _context.Institutions
            .Include(i => i.Services)
            .FirstOrDefault(i => i.Id == institutionId)
            ?? throw new KeyNotFoundException("Institution not found.");

        institution.AddService(new MedicalService(name, category, TimeSpan.FromMinutes(durationMinutes), price));
        _context.SaveChanges();
    }

    public void UpdateService(int serviceId, string name, ServiceCategory category, int durationMinutes, decimal price)
    {
        name = RequireText(name, "Service name");

        var service = _context.MedicalServices.Find(serviceId)
            ?? throw new KeyNotFoundException("Service not found.");

        if (_context.MedicalServices.Any(s => s.Id != serviceId
                && s.InstitutionId == service.InstitutionId
                && s.Name.ToLower() == name.ToLower()))
            throw new InvalidOperationException($"A service named '{name}' already exists.");

        service.UpdateDetails(name, category);
        service.UpdatePricing(TimeSpan.FromMinutes(durationMinutes), price);
        _context.SaveChanges();
    }

    private Branch LoadBranch(int branchId)
    {
        return _context.Branches
            .Include(b => b.Departments)
            .Include(b => b.Schedule)
            .FirstOrDefault(b => b.Id == branchId)
            ?? throw new KeyNotFoundException("Branch not found.");
    }

    private static string RequireText(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} is required.");
        return value.Trim();
    }
}