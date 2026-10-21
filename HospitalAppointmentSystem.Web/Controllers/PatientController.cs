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
            .Include(a => a.Doctor).ThenInclude(d => d!.Branch)
            .Include(a => a.MedicalService)
            .Where(a => a.PatientId == CurrentPatientId)
            .OrderByDescending(a => a.StartTime)
            .ToList();

        return View(appointments);
    }

    [HttpGet]
    public IActionResult Book(int? branchId, int? serviceId, int? doctorId, DateTime? date)
    {
        var model = new BookAppointmentViewModel
        {
            Branches = _context.Branches.OrderBy(b => b.Name).ToList()
        };

        if (branchId != null && model.Branches.Any(b => b.Id == branchId))
        {
            model.BranchId = branchId;
            model.Services = _context.MedicalServices
                .Where(s => s.Doctors.Any(d => d.BranchId == branchId))
                .OrderBy(s => s.Name)
                .ToList();
        }

        if (model.BranchId != null && serviceId != null && model.Services.Any(s => s.Id == serviceId))
        {
            model.ServiceId = serviceId;
            model.SelectedService = model.Services.First(s => s.Id == serviceId);
            model.Doctors = _context.Doctors
                .Where(d => d.BranchId == branchId && d.Services.Any(s => s.Id == serviceId))
                .OrderBy(d => d.LastName)
                .ToList();
        }

        if (model.ServiceId != null && doctorId != null && model.Doctors.Any(d => d.Id == doctorId))
        {
            model.DoctorId = doctorId;
            model.Date = date?.Date < DateTime.Today ? DateTime.Today : date?.Date ?? DateTime.Today.AddDays(1);
            model.SelectedDoctor = LoadDoctor(doctorId.Value);
            model.AvailableTimes = _appointmentService.GetAvailableStartTimes(
                model.SelectedDoctor!, model.SelectedService!, model.Date.Value);
        }

        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Book(int doctorId, int serviceId, DateTime startTime)
    {
        var patient = _context.Patients.Find(CurrentPatientId);
        var doctor = LoadDoctor(doctorId);
        var service = _context.MedicalServices.Find(serviceId);

        if (patient == null || doctor == null || service == null)
            return NotFound();

        try
        {
            _appointmentService.Book(patient, doctor, service, startTime);
            TempData["Success"] = $"Randevu talebiniz oluşturuldu ({startTime:dd MMMM yyyy HH:mm}, Dr. {doctor.FullName}). Doktor onayı bekleniyor.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex) when (ex is AppointmentConflictException or InvalidOperationException or ArgumentException)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Book), new
            {
                branchId = doctor.BranchId,
                serviceId,
                doctorId,
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

    private Doctor? LoadDoctor(int id)
    {
        return _context.Doctors
            .Include(d => d.Branch).ThenInclude(b => b!.Schedule)
            .Include(d => d.Availability)
            .Include(d => d.Services)
            .FirstOrDefault(d => d.Id == id);
    }
}