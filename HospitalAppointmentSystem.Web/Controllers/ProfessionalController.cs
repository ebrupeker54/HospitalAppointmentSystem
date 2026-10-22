using HospitalAppointmentSystem.Web.Models;
using System.Security.Claims;
using HospitalAppointmentSystem.Web.Data;
using HospitalAppointmentSystem.Web.Services;
using HospitalAppointmentSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Controllers;

[Authorize(Roles = "Doctor,HealthStaff")]
public class ProfessionalController : Controller
{
    private readonly AppDbContext _context;
    private readonly AppointmentService _appointmentService;
    private readonly AvailabilityService _availabilityService;
    private readonly ResultService _resultService;

    public ProfessionalController(AppDbContext context, AppointmentService appointmentService,
        AvailabilityService availabilityService, ResultService resultService)
    {
        _context = context;
        _appointmentService = appointmentService;
        _availabilityService = availabilityService;
        _resultService = resultService;
    }

    private int CurrentProfessionalId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private HealthcareProfessional CurrentProfessional =>
    _context.HealthcareProfessionals.Find(CurrentProfessionalId)!;
    public IActionResult Index()
    {
        var model = new ProfessionalDashboardViewModel
        {
            Appointments = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.MedicalService)
                .Include(a => a.Result)
                .Where(a => a.ProfessionalId == CurrentProfessionalId)
                .OrderBy(a => a.StartTime)
                .ToList(),
            UpcomingSlots = _context.AvailabilitySlots
                .Where(s => s.ProfessionalId == CurrentProfessionalId && s.End > DateTime.Now)
                .OrderBy(s => s.Start)
                .Take(30)
                .ToList(),
            CanEnterResults = _context.HealthcareProfessionals.Find(CurrentProfessionalId) is IResultEntry
        };

        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Approve(int id) =>
    Handle(() => _appointmentService.Approve(id, CurrentProfessional), "Randevu onaylandı.");

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Reject(int id, string? reason) =>
        Handle(() => _appointmentService.Reject(id, CurrentProfessional, reason ?? string.Empty), "Randevu reddedildi.");

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Complete(int id) =>
        Handle(() => _appointmentService.Complete(id, CurrentProfessional), "Randevu tamamlandı olarak işaretlendi.");

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult AddAvailability(DateTime date, TimeSpan startTime, TimeSpan endTime) =>
        Handle(() => _availabilityService.AddSlot(CurrentProfessional, CurrentProfessionalId,
            date.Date + startTime, date.Date + endTime), "Uygunluk aralığı eklendi.");

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult RemoveAvailability(int id) =>
        Handle(() => _availabilityService.RemoveSlot(CurrentProfessional, id), "Uygunluk aralığı silindi.");

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult EnterResult(int id, string? findings) =>
    Handle(() => _resultService.EnterResult(id, CurrentProfessionalId, findings), "Sonuç kaydedildi.");

    private IActionResult Handle(Action action, string successMessage)
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

        return RedirectToAction(nameof(Index));
    }
}