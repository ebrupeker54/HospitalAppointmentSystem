using System.ComponentModel.DataAnnotations;

namespace HospitalAppointmentSystem.Web.ViewModels;

public class RegisterViewModel
{
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

    [Required(ErrorMessage = "T.C. kimlik numarası zorunludur.")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "T.C. kimlik numarası 11 haneli olmalıdır.")]
    [Display(Name = "T.C. Kimlik No")]
    public string NationalId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Doğum tarihi zorunludur.")]
    [DataType(DataType.Date)]
    [Display(Name = "Doğum Tarihi")]
    public DateTime? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Şifreler eşleşmiyor.")]
    [Display(Name = "Şifre (Tekrar)")]
    public string ConfirmPassword { get; set; } = string.Empty;
}