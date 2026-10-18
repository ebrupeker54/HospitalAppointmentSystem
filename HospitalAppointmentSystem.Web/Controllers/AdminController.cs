using HospitalAppointmentSystem.Web.Data;
using HospitalAppointmentSystem.Web.Models;
using HospitalAppointmentSystem.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalAppointmentSystem.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
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
}