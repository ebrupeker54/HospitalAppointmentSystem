namespace HospitalAppointmentSystem.Web.Models;

public static class StaffFactory
{
    public static Staff Create(StaffType type) => type switch
    {
        StaffType.Doctor => new Doctor(),
        StaffType.Nurse => new Nurse(),
        StaffType.RadiologyTechnician => new RadiologyTechnician(),
        StaffType.LabTechnician => new LabTechnician(),
        StaffType.Secretary => new Secretary(),
        _ => throw new ArgumentException("Geçersiz personel türü.")
    };

    public static StaffType TypeOf(Staff staff) => staff switch
    {
        Doctor => StaffType.Doctor,
        Nurse => StaffType.Nurse,
        RadiologyTechnician => StaffType.RadiologyTechnician,
        LabTechnician => StaffType.LabTechnician,
        Secretary => StaffType.Secretary,
        _ => throw new ArgumentException("Bilinmeyen personel türü.")
    };
}