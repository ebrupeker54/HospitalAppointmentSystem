using HospitalAppointmentSystem.Web.Models;

namespace HospitalAppointmentSystem.Web.Helpers;

public static class StatusBadgeExtensions
{
    public static string ToBadgeClass(this AppointmentStatus status) => status switch
    {
        AppointmentStatus.Pending => "bg-warning text-dark",
        AppointmentStatus.Approved => "bg-success",
        AppointmentStatus.Rejected => "bg-danger",
        AppointmentStatus.Cancelled => "bg-secondary",
        AppointmentStatus.Completed => "bg-primary",
        _ => "bg-light text-dark"
    };
}