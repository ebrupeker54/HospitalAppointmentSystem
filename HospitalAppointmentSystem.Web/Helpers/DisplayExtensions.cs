using System.Globalization;
using HospitalAppointmentSystem.Web.Models;

namespace HospitalAppointmentSystem.Web.Helpers;

public static class DisplayExtensions
{
    private static readonly CultureInfo Turkish = new("tr-TR");

    public static string ToDisplay(this AppointmentStatus status) => status switch
    {
        AppointmentStatus.Pending => "Onay Bekliyor",
        AppointmentStatus.Approved => "Onaylandı",
        AppointmentStatus.Rejected => "Reddedildi",
        AppointmentStatus.Cancelled => "İptal Edildi",
        AppointmentStatus.Completed => "Tamamlandı",
        _ => status.ToString()
    };

    public static string ToDisplay(this UserRole role) => role switch
    {
        UserRole.Patient => "Hasta",
        UserRole.Doctor => "Doktor",
        UserRole.Admin => "Yönetici",
        UserRole.HealthStaff => "Sağlık Personeli",
        UserRole.Secretary => "Sekreter",
        _ => role.ToString()
    };

    public static string ToDisplay(this ServiceCategory category) => category switch
    {
        ServiceCategory.GeneralExamination => "Genel Muayene",
        ServiceCategory.Radiology => "Radyoloji",
        ServiceCategory.Laboratory => "Laboratuvar",
        ServiceCategory.Nursing => "Hemşirelik",
        ServiceCategory.Other => "Diğer",
        _ => category.ToString()
    };

    public static string ToDisplay(this InstitutionType type) => type switch
    {
        InstitutionType.Hospital => "Hastane",
        InstitutionType.Clinic => "Klinik",
        _ => type.ToString()
    };

    public static string ToDisplay(this StaffType type) => type switch
    {
        StaffType.Doctor => "Doktor",
        StaffType.Nurse => "Hemşire",
        StaffType.RadiologyTechnician => "Radyoloji Teknikeri",
        StaffType.LabTechnician => "Laborant",
        StaffType.Secretary => "Sekreter",
        _ => type.ToString()
    };
    public static string ToDisplay(this DayOfWeek day) => Turkish.DateTimeFormat.GetDayName(day);

    public static string RoleDisplay(string? role) =>
        Enum.TryParse<UserRole>(role, out var parsed) ? parsed.ToDisplay() : role ?? string.Empty;
}