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

    public AdminController(AppDbContext context, InstitutionService institutionService)
    {
        _context = context;
        _institutionService = institutionService;
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
            "Institution created.", RedirectToAction(nameof(Institutions)));

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult UpdateInstitution(int id, string name, InstitutionType type) =>
        Handle(() => _institutionService.UpdateInstitution(id, name, type),
            "Institution updated.", RedirectToAction(nameof(Institutions)));

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult CreateBranch(int institutionId, string name, string? address, string? phoneNumber) =>
        Handle(() => _institutionService.AddBranch(institutionId, name, address, phoneNumber),
            "Branch added.", RedirectToAction(nameof(Institutions)));

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
            "Branch updated.", RedirectToAction(nameof(Branch), new { id }));

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult AddDepartment(int branchId, string name) =>
        Handle(() => _institutionService.AddDepartment(branchId, name),
            "Department added.", RedirectToAction(nameof(Branch), new { id = branchId }));

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult RemoveDepartment(int branchId, int departmentId) =>
        Handle(() => _institutionService.RemoveDepartment(branchId, departmentId),
            "Department removed.", RedirectToAction(nameof(Branch), new { id = branchId }));

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult UpdateSchedule(int branchId, List<DayScheduleInput> schedule) =>
        Handle(() => _institutionService.UpdateSchedule(branchId, schedule),
            "Working hours saved.", RedirectToAction(nameof(Branch), new { id = branchId }));

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