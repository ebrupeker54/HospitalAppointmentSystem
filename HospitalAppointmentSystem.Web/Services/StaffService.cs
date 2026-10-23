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

    public void CreateStaff(StaffFormViewModel form)
    {
        var email = NormalizeEmail(form.Email);
        if (_context.Users.Any(u => u.Email == email))
            throw new InvalidOperationException("Bu e-posta adresiyle kayıtlı bir hesap zaten var.");

        var staff = StaffFactory.Create(form.Type);
        SetBasicInfo(staff, form, email);
        staff.SetPassword(form.Password ?? string.Empty);

        ApplyAssignments(staff, form);

        _context.Users.Add(staff);
        _context.SaveChanges();
    }

    public void UpdateStaff(StaffFormViewModel form)
    {
        var staff = LoadStaff(form.Id ?? 0)
            ?? throw new KeyNotFoundException("Personel bulunamadı.");

        var email = NormalizeEmail(form.Email);
        if (_context.Users.Any(u => u.Id != staff.Id && u.Email == email))
            throw new InvalidOperationException("Bu e-posta adresiyle kayıtlı bir hesap zaten var.");

        SetBasicInfo(staff, form, email);
        ApplyAssignments(staff, form);
        _context.SaveChanges();
    }

    public Staff? LoadStaff(int id)
    {
        var staff = _context.Users.Find(id) as Staff;

        if (staff is HealthcareProfessional)
            _context.HealthcareProfessionals.Include(p => p.Services).Where(p => p.Id == id).Load();
        else if (staff is Secretary)
            _context.Secretaries.Include(s => s.Doctors).Where(s => s.Id == id).Load();

        return staff;
    }

    private static void SetBasicInfo(Staff staff, StaffFormViewModel form, string email)
    {
        staff.FirstName = form.FirstName.Trim();
        staff.LastName = form.LastName.Trim();
        staff.Email = email;
        staff.PhoneNumber = form.PhoneNumber.Trim();
    }

    private void ApplyAssignments(Staff staff, StaffFormViewModel form)
    {
        var branch = _context.Branches
            .Include(b => b.Institution)
            .Include(b => b.Departments)
            .FirstOrDefault(b => b.Id == form.BranchId)
            ?? throw new ArgumentException("Lütfen bir şube seçin.");

        var department = form.DepartmentId == null
            ? null
            : branch.Departments.FirstOrDefault(d => d.Id == form.DepartmentId)
              ?? throw new InvalidOperationException("Seçilen bölüm, seçilen şubeye ait değil.");

        staff.AssignTo(branch, department);

        if (staff is Doctor doctor)
        {
            if (string.IsNullOrWhiteSpace(form.Specialty))
                throw new ArgumentException("Doktorlar için uzmanlık alanı zorunludur.");
            doctor.Specialty = form.Specialty.Trim();
        }

        if (staff is HealthcareProfessional professional)
        {
            var serviceIds = form.ServiceIds.Distinct().ToList();
            var services = _context.MedicalServices
                .Where(s => serviceIds.Contains(s.Id) && s.InstitutionId == branch.InstitutionId)
                .ToList();

            if (services.Count != serviceIds.Count)
                throw new InvalidOperationException("Personel yalnızca şubesinin bağlı olduğu kurumun hizmetlerini verebilir.");

            professional.SetServices(services);
        }

        if (staff is Secretary secretary)
        {
            var doctorIds = form.DoctorIds.Distinct().ToList();
            var doctors = _context.Doctors
                .Include(d => d.Branch)
                .Where(d => doctorIds.Contains(d.Id))
                .ToList();

            secretary.SetDoctors(doctors);
        }
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}