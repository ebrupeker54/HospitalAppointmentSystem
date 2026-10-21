using System.ComponentModel.DataAnnotations;
using HospitalAppointmentSystem.Web.Models;

namespace HospitalAppointmentSystem.Web.ViewModels;

public class DoctorFormViewModel
{
    public int? Id { get; set; }

    [Required, Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required, Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, Phone, Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public string Specialty { get; set; } = string.Empty;

    [DataType(DataType.Password), MinLength(6)]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Please select a branch."), Display(Name = "Branch")]
    public int? BranchId { get; set; }

    [Display(Name = "Department")]
    public int? DepartmentId { get; set; }

    public List<int> ServiceIds { get; set; } = new();

    public List<Branch> Branches { get; set; } = new();
    public List<MedicalService> Services { get; set; } = new();

    public bool IsEdit => Id != null;
}