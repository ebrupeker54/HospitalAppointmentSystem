using System.ComponentModel.DataAnnotations;
using HospitalAppointmentSystem.Web.Models;

namespace HospitalAppointmentSystem.Web.ViewModels;

public class StaffFormViewModel
{
    public int? Id { get; set; }

    [Display(Name = "Personel Türü")]
    public StaffType Type { get; set; } = StaffType.Doctor;

    [Required(ErrorMessage = "Ad zorunludur.")]
    [Display(Name = "Ad")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [Display(Name = "Soyad")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [Display(Name = "E-posta")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon numarası zorunludur.")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
    [Display(Name = "Telefon")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Display(Name = "Uzmanlık Alanı")]
    public string? Specialty { get; set; }

    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    [DataType(DataType.Password)]
    [Display(Name = "Başlangıç Şifresi")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Lütfen bir şube seçin.")]
    [Display(Name = "Şube")]
    public int? BranchId { get; set; }

    [Display(Name = "Bölüm")]
    public int? DepartmentId { get; set; }

    public List<int> ServiceIds { get; set; } = new();
    public List<int> DoctorIds { get; set; } = new();

    public List<Branch> Branches { get; set; } = new();
    public List<MedicalService> Services { get; set; } = new();
    public List<Doctor> Doctors { get; set; } = new();

    public bool IsEdit => Id != null;
}