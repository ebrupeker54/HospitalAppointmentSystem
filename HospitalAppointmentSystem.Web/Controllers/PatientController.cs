using System.Security.Claims;
using HospitalAppointmentSystem.Web.Data;
using HospitalAppointmentSystem.Web.Models;
using HospitalAppointmentSystem.Web.Services;
using HospitalAppointmentSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Controllers;

[Authorize(Roles = "Patient")]
public class PatientController : Controller
{
    private readonly AppDbContext _context;
    private readonly AppointmentService _appointmentService;

    public PatientController(AppDbContext context, AppointmentService appointmentService)
    {
        _context = context;
        _appointmentService = appointmentService;
    }

    private int CurrentPatientId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public IActionResult Index()
    {
        var appointments = _context.Appointments
            .Include(a => a.Professional).ThenInclude(p => p!.Branch)
            .Include(a => a.MedicalService)
            .Where(a => a.PatientId == CurrentPatientId)
            .OrderByDescending(a => a.StartTime)
            .ToList();

        return View(appointments);
    }

    [HttpGet]
    public IActionResult Book(int? branchId, int? serviceId, int? professionalId, DateTime? date)
    {
        var model = new BookAppointmentViewModel
        {
            Branches = _context.Branches.OrderBy(b => b.Name).ToList()
        };

        if (branchId != null && model.Branches.Any(b => b.Id == branchId))
        {
            model.BranchId = branchId;
            model.Services = _context.MedicalServices
                .Where(s => s.Professionals.Any(p => p.BranchId == branchId))
                .OrderBy(s => s.Name)
                .ToList();
        }

        if (model.BranchId != null && serviceId != null && model.Services.Any(s => s.Id == serviceId))
        {
            model.ServiceId = serviceId;
            model.SelectedService = model.Services.First(s => s.Id == serviceId);
            model.Professionals = _context.HealthcareProfessionals
                .Where(p => p.BranchId == branchId && p.Services.Any(s => s.Id == serviceId))
                .OrderBy(p => p.LastName)
                .ToList();
        }

        if (model.ServiceId != null && professionalId != null && model.Professionals.Any(p => p.Id == professionalId))
        {
            model.ProfessionalId = professionalId;
            model.Date = date?.Date < DateTime.Today ? DateTime.Today : date?.Date ?? DateTime.Today.AddDays(1);
            model.SelectedProfessional = LoadProfessional(professionalId.Value);
            model.AvailableTimes = _appointmentService.GetAvailableStartTimes(
                model.SelectedProfessional!, model.SelectedService!, model.Date.Value);
        }

        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Book(int professionalId, int serviceId, DateTime startTime)
    {
        var patient = _context.Patients.Find(CurrentPatientId);
        var professional = LoadProfessional(professionalId);
        var service = _context.MedicalServices.Find(serviceId);

        if (patient == null || professional == null || service == null)
            return NotFound();

        try
        {
            _appointmentService.Book(patient, professional, service, startTime);
            TempData["Success"] = $"Randevu talebiniz oluşturuldu ({startTime:dd MMMM yyyy HH:mm}, {professional.DisplayName}). Onay bekleniyor.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (ex is AppointmentConflictException or InvalidOperationException or ArgumentException)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Book), new
            {
                branchId = professional.BranchId,
                serviceId,
                professionalId,
                date = startTime.ToString("yyyy-MM-dd")
            });
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Cancel(int id)
    {
        try
        {
            _appointmentService.CancelByPatient(id, CurrentPatientId);
            TempData["Success"] = "Randevunuz iptal edildi.";
        }
        catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private HealthcareProfessional? LoadProfessional(int id)
    {
        return _context.HealthcareProfessionals
            .Include(p => p.Branch).ThenInclude(b => b!.Schedule)
            .Include(p => p.Availability)
            .Include(p => p.Services)
            .FirstOrDefault(p => p.Id == id);
    }
}