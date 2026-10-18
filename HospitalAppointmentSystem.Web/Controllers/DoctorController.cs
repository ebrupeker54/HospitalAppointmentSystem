using System.Security.Claims;
using HospitalAppointmentSystem.Web.Data;
using HospitalAppointmentSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Controllers;

[Authorize(Roles = "Doctor")]
public class DoctorController : Controller
{
    private readonly AppDbContext _context;

    public DoctorController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var doctorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var model = new DoctorDashboardViewModel
        {
            Appointments = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.MedicalService)
                .Where(a => a.DoctorId == doctorId)
                .OrderBy(a => a.StartTime)
                .ToList(),
            UpcomingSlots = _context.AvailabilitySlots
                .Where(s => s.DoctorId == doctorId && s.End > DateTime.Now)
                .OrderBy(s => s.Start)
                .Take(10)
                .ToList()
        };

        return View(model);
    }
}