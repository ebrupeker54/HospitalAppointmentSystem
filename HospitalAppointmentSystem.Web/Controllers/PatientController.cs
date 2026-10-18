using System.Security.Claims;
using HospitalAppointmentSystem.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Controllers;

[Authorize(Roles = "Patient")]
public class PatientController : Controller
{
    private readonly AppDbContext _context;

    public PatientController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var patientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var appointments = _context.Appointments
            .Include(a => a.Doctor).ThenInclude(d => d!.Branch)
            .Include(a => a.MedicalService)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.StartTime)
            .ToList();

        return View(appointments);
    }
}