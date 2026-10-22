using HospitalAppointmentSystem.Web.Data;
using HospitalAppointmentSystem.Web.Models;
using HospitalAppointmentSystem.Web.Services;
using HospitalAppointmentSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private static readonly DayOfWeek[] WeekDays =
    {
        DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday,
        DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday
    };

    private readonly AppDbContext _context;
    private readonly InstitutionService _institutionService;
    private readonly StaffService _staffService;

    public AdminController(AppDbContext context, InstitutionService institutionService, StaffService staffService)
    {
        _context = context;
        _institutionService = institutionService;
        _staffService = staffService;
    }

    public IActionResult Index()
    {
        var model = new AdminDashboardViewModel
        {
            InstitutionCount = _context.Institutions.Count(),
            BranchCount = _context.Branches.Count(),
            DoctorCount = _context.Doctors.Count(),
            PatientCount = _context.Patients.Count(),
            PendingAppointmentCount = _context.Appointments.Count(a => a.Status == AppointmentStatus.Pending),
            Doctors = _context.Doctors
                .Include(d => d.Branch)
                .Include(d => d.Services)
                .OrderBy(d => d.LastName)
                .ToList()
        };

        return View(model);
    }

    // ---------- Institutions ----------

    public IActionResult Institutions()
    {
        var institutions = _context.Institutions
            .Include(i => i.Branches)
            .OrderBy(i => i.Name)
            .ToList();

        return View(institutions);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult CreateInstitution(string name, InstitutionType type) =>
        Handle(() => _institutionService.CreateInstitution(name, type),
            "Kurum oluşturuldu.", RedirectToAction(nameof(Institutions)));

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult UpdateInstitution(int id, string name, InstitutionType type) =>
        Handle(() => _institutionService.UpdateInstitution(id, name, type),
            "Kurum güncellendi.", RedirectToAction(nameof(Institutions)));

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult CreateBranch(int institutionId, string name, string? address, string? phoneNumber) =>
        Handle(() => _institutionService.AddBranch(institutionId, name, address, phoneNumber),
            "Şube eklendi.", RedirectToAction(nameof(Institutions)));

    // ---------- Branch details ----------

    public IActionResult Branch(int id)
    {
        var branch = _context.Branches
            .Include(b => b.Institution)
            .Include(b => b.Departments)
            .Include(b => b.Schedule)
            .FirstOrDefault(b => b.Id == id);

        if (branch == null)
            return NotFound();

        var model = new BranchDetailsViewModel
        {
            Branch = branch,
            Schedule = WeekDays.Select(day =>
            {
                var hours = branch.Schedule.FirstOrDefault(w => w.Day == day);
                return new DayScheduleInput
                {
                    Day = day,
                    IsOpen = hours != null,
                    OpenTime = hours?.OpenTime.ToTimeSpan() ?? new TimeSpan(9, 0, 0),
                    CloseTime = hours?.CloseTime.ToTimeSpan() ?? new TimeSpan(17, 0, 0)
                };
            }).ToList(),
            DoctorCounts = _context.Doctors
                .Where(d => d.BranchId == id && d.DepartmentId != null)
                .GroupBy(d => d.DepartmentId!.Value)
                .ToDictionary(g => g.Key, g => g.Count())
        };

        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult UpdateBranch(int id, string name, string? address, string? phoneNumber) =>
        Handle(() => _institutionService.UpdateBranch(id, name, address, phoneNumber),
            "Şube güncellendi.", RedirectToAction(nameof(Branch), new { id }));

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult AddDepartment(int branchId, string name) =>
        Handle(() => _institutionService.AddDepartment(branchId, name),
            "Bölüm eklendi.", RedirectToAction(nameof(Branch), new { id = branchId }));

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult RemoveDepartment(int branchId, int departmentId) =>
        Handle(() => _institutionService.RemoveDepartment(branchId, departmentId),
            "Bölüm silindi.", RedirectToAction(nameof(Branch), new { id = branchId }));

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult UpdateSchedule(int branchId, List<DayScheduleInput> schedule) =>
        Handle(() => _institutionService.UpdateSchedule(branchId, schedule),
            "Çalışma saatleri kaydedildi.", RedirectToAction(nameof(Branch), new { id = branchId }));

    // ---------- Services ----------

    public IActionResult Services()
    {
        var institutions = _context.Institutions
                        .Include(i => i.Services).ThenInclude(s => s.Professionals)
            .OrderBy(i => i.Name)
            .ToList();

        return View(institutions);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult CreateService(int institutionId, string name, ServiceCategory category, int durationMinutes, decimal price) =>
        Handle(() => _institutionService.AddService(institutionId, name, category, durationMinutes, price),
            "Hizmet eklendi.", RedirectToAction(nameof(Services)));

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult UpdateService(int id, string name, ServiceCategory category, int durationMinutes, decimal price) =>
        Handle(() => _institutionService.UpdateService(id, name, category, durationMinutes, price),
            "Hizmet güncellendi.", RedirectToAction(nameof(Services)));

    // ---------- Doctors ----------

    public IActionResult Doctors()
    {
        var doctors = _context.Doctors
            .Include(d => d.Branch).ThenInclude(b => b!.Institution)
            .Include(d => d.Department)
            .Include(d => d.Services)
            .OrderBy(d => d.LastName)
            .ToList();

        return View(doctors);
    }

    [HttpGet]
    public IActionResult CreateDoctor()
    {
        var model = new DoctorFormViewModel();
        FillDoctorFormLists(model);
        return View("DoctorForm", model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult CreateDoctor(DoctorFormViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Password))
            ModelState.AddModelError(nameof(model.Password), "Şifre zorunludur.");

        return SaveDoctor(model, () => _staffService.CreateDoctor(model), "Doktor kaydı oluşturuldu..");
    }

    [HttpGet]
    public IActionResult EditDoctor(int id)
    {
        var doctor = _context.Doctors.Include(d => d.Services).FirstOrDefault(d => d.Id == id);
        if (doctor == null)
            return NotFound();

        var model = new DoctorFormViewModel
        {
            Id = doctor.Id,
            FirstName = doctor.FirstName,
            LastName = doctor.LastName,
            Email = doctor.Email,
            PhoneNumber = doctor.PhoneNumber,
            Specialty = doctor.Specialty,
            BranchId = doctor.BranchId,
            DepartmentId = doctor.DepartmentId,
            ServiceIds = doctor.Services.Select(s => s.Id).ToList()
        };
        FillDoctorFormLists(model);
        return View("DoctorForm", model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult EditDoctor(DoctorFormViewModel model) =>
        SaveDoctor(model, () => _staffService.UpdateDoctor(model), "Doktor bilgileri güncellendi.");

    private IActionResult SaveDoctor(DoctorFormViewModel model, Action save, string successMessage)
    {
        if (ModelState.IsValid)
        {
            try
            {
                save();
                TempData["Success"] = successMessage;
                return RedirectToAction(nameof(Doctors));
            }
            catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or KeyNotFoundException)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        FillDoctorFormLists(model);
        return View("DoctorForm", model);
    }

    private void FillDoctorFormLists(DoctorFormViewModel model)
    {
        model.Branches = _context.Branches
            .Include(b => b.Institution)
            .Include(b => b.Departments)
            .OrderBy(b => b.Institution!.Name).ThenBy(b => b.Name)
            .ToList();
        model.Services = _context.MedicalServices
            .Include(s => s.Institution)
            .OrderBy(s => s.Name)
            .ToList();
    }

    private IActionResult Handle(Action action, string successMessage, IActionResult redirect)
    {
        try
        {
            action();
            TempData["Success"] = successMessage;
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or KeyNotFoundException)
        {
            TempData["Error"] = ex.Message;
        }

        return redirect;
    }
}