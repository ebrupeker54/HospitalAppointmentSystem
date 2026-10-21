namespace HospitalAppointmentSystem.Web.Models;

public class Admin : User
{
    public Admin()
    {
        Role = UserRole.Admin;
    }

    public override string DashboardController => "Admin";
    public override string GetRoleDescription()
    {
        return "Yönetici - kurumları, şubeleri, hizmetleri ve personeli yönetebilir";
    }
}