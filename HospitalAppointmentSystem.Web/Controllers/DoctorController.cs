using System.Security.Claims;
using HospitalAppointmentSystem.Web.Data;
using HospitalAppointmentSystem.Web.Services;
using HospitalAppointmentSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Controllers;

[Authorize(Roles = "Doctor")]
public class DoctorController : Controller
{
    private readonly AppDbContext _context;
    private readonly AppointmentService _appointmentService;
    private readonly AvailabilityService _availabilityService;

    public DoctorController(AppDbContext context, AppointmentService appointmentService,
        AvailabilityService availabilityService)
    {
        _context = context;
        _appointmentService = appointmentService;
        _availabilityService = availabilityService;
    }

    private int CurrentDoctorId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public IActionResult Index()
    {
        var model = new DoctorDashboardViewModel
        {
            Appointments = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.MedicalService)
                .Where(a => a.DoctorId == CurrentDoctorId)
                .OrderBy(a => a.StartTime)
                .ToList(),
            UpcomingSlots = _context.AvailabilitySlots
                .Where(s => s.DoctorId == CurrentDoctorId && s.End > DateTime.Now)
                .OrderBy(s => s.Start)
                .Take(30)
                .ToList()
        };

        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Approve(int id) =>
        Handle(() => _appointmentService.ApproveByDoctor(id, CurrentDoctorId), "Appointment approved.");

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Reject(int id, string? reason) =>
        Handle(() => _appointmentService.RejectByDoctor(id, CurrentDoctorId, reason ?? string.Empty), "Appointment rejected.");

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Complete(int id) =>
        Handle(() => _appointmentService.CompleteByDoctor(id, CurrentDoctorId), "Appointment marked as completed.");

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult AddAvailability(DateTime date, TimeSpan startTime, TimeSpan endTime) =>
        Handle(() => _availabilityService.AddSlot(CurrentDoctorId, date.Date + startTime, date.Date + endTime),
            "Availability slot added.");

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult RemoveAvailability(int id) =>
        Handle(() => _availabilityService.RemoveSlot(id, CurrentDoctorId), "Availability slot removed.");

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