using System.Security.Claims;
using HospitalAppointmentSystem.Web.Data;
using HospitalAppointmentSystem.Web.Models;
using HospitalAppointmentSystem.Web.Services;
using HospitalAppointmentSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Controllers;

[Authorize(Roles = "Secretary")]
public class SecretaryController : Controller
{
    private readonly AppDbContext _context;
    private readonly AppointmentService _appointmentService;
    private readonly AvailabilityService _availabilityService;

    public SecretaryController(AppDbContext context, AppointmentService appointmentService,
        AvailabilityService availabilityService)
    {
        _context = context;
        _appointmentService = appointmentService;
        _availabilityService = availabilityService;
    }

    private int CurrentSecretaryId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public IActionResult Index(int? doctorId)
    {
        var secretary = LoadSecretary();
        var managedIds = secretary.Doctors.Select(d => d.Id).ToList();

        if (doctorId != null && !managedIds.Contains(doctorId.Value))
            doctorId = null;

        var filterIds = doctorId != null ? new List<int> { doctorId.Value } : managedIds;

        var model = new SecretaryDashboardViewModel
        {
            Doctors = secretary.Doctors.OrderBy(d => d.LastName).ToList(),
            SelectedDoctorId = doctorId,
            Appointments = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Professional)
                .Include(a => a.MedicalService)
                .Where(a => filterIds.Contains(a.ProfessionalId) && a.EndTime >= DateTime.Today)
                .OrderBy(a => a.StartTime)
                .ToList(),
            Slots = doctorId == null
                ? new List<AvailabilitySlot>()
                : _context.AvailabilitySlots
                    .Where(s => s.ProfessionalId == doctorId && s.End > DateTime.Now)
                    .OrderBy(s => s.Start)
                    .Take(30)
                    .ToList()
        };

        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Approve(int id, int? doctorId) =>
        Handle(() => _appointmentService.Approve(id, LoadSecretary()), "Randevu onaylandı.", doctorId);

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Reject(int id, string? reason, int? doctorId) =>
        Handle(() => _appointmentService.Reject(id, LoadSecretary(), reason ?? string.Empty), "Randevu reddedildi.", doctorId);

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult AddAvailability(int doctorId, DateTime date, TimeSpan startTime, TimeSpan endTime) =>
        Handle(() => _availabilityService.AddSlot(LoadSecretary(), doctorId, date.Date + startTime, date.Date + endTime),
            "Uygunluk aralığı eklendi.", doctorId);

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult RemoveAvailability(int id, int doctorId) =>
        Handle(() => _availabilityService.RemoveSlot(LoadSecretary(), id), "Uygunluk aralığı silindi.", doctorId);

    [HttpGet]
    public IActionResult Book(string? nationalId, int? doctorId, int? serviceId, DateTime? date)
    {
        var secretary = LoadSecretary();
        var model = new SecretaryBookViewModel
        {
            NationalId = nationalId,
            Doctors = secretary.Doctors.OrderBy(d => d.LastName).ToList()
        };

        if (!string.IsNullOrWhiteSpace(nationalId))
        {
            model.Patient = _context.Patients.FirstOrDefault(p => p.NationalId == nationalId.Trim());
            model.PatientNotFound = model.Patient == null;
        }

        if (model.Patient != null && doctorId != null && secretary.CanManage(doctorId.Value))
        {
            model.DoctorId = doctorId;
            model.SelectedDoctor = LoadProfessional(doctorId.Value);
            model.Services = model.SelectedDoctor!.Services.OrderBy(s => s.Name).ToList();
        }

        if (model.SelectedDoctor != null && serviceId != null && model.Services.Any(s => s.Id == serviceId))
        {
            model.ServiceId = serviceId;
            model.SelectedService = model.Services.First(s => s.Id == serviceId);
            model.Date = date?.Date < DateTime.Today ? DateTime.Today : date?.Date ?? DateTime.Today.AddDays(1);
            model.AvailableTimes = _appointmentService.GetAvailableStartTimes(
                model.SelectedDoctor, model.SelectedService, model.Date.Value);
        }

        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Book(int patientId, int doctorId, int serviceId, DateTime startTime, string nationalId)
    {
        var secretary = LoadSecretary();
        var patient = _context.Patients.Find(patientId);
        var doctor = LoadProfessional(doctorId);
        var service = _context.MedicalServices.Find(serviceId);

        if (patient == null || doctor == null || service == null || !secretary.CanManage(doctorId))
            return NotFound();

        try
        {
            var appointment = _appointmentService.Book(patient, doctor, service, startTime);
            _appointmentService.Approve(appointment.Id, secretary);

            TempData["Success"] = $"{patient.FullName} için {startTime:dd MMMM yyyy HH:mm} tarihli randevu oluşturuldu ve onaylandı.";
            return RedirectToAction(nameof(Index), new { doctorId });
        }
        catch (Exception ex) when (ex is AppointmentConflictException or InvalidOperationException or ArgumentException)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Book), new
            {
                nationalId,
                doctorId,
                serviceId,
                date = startTime.ToString("yyyy-MM-dd")
            });
        }
    }

    private Secretary LoadSecretary()
    {
        return _context.Secretaries
            .Include(s => s.Doctors)
            .First(s => s.Id == CurrentSecretaryId);
    }

    private HealthcareProfessional? LoadProfessional(int id)
    {
        return _context.HealthcareProfessionals
            .Include(p => p.Branch).ThenInclude(b => b!.Schedule)
            .Include(p => p.Availability)
            .Include(p => p.Services)
            .FirstOrDefault(p => p.Id == id);
    }

    private IActionResult Handle(Action action, string successMessage, int? doctorId)
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

        return RedirectToAction(nameof(Index), new { doctorId });
    }
}