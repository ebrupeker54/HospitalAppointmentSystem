using HospitalAppointmentSystem.Web.Models;

namespace HospitalAppointmentSystem.Web.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (context.Institutions.Any())
            return;

        var institution = new Institution { Name = "City Health Hospital", Type = InstitutionType.Hospital };

        var central = new Branch { Name = "Central Branch", Address = "Main Street No:1", PhoneNumber = "0264 000 00 01" };
        var north = new Branch { Name = "North Branch", Address = "North Avenue No:25", PhoneNumber = "0264 000 00 02" };
        institution.AddBranch(central);
        institution.AddBranch(north);

        foreach (var branch in new[] { central, north })
        {
            for (var day = DayOfWeek.Monday; day <= DayOfWeek.Friday; day++)
                branch.SetWorkingHours(new WorkingHours(day, new TimeOnly(8, 0), new TimeOnly(18, 0)));
        }
        central.SetWorkingHours(new WorkingHours(DayOfWeek.Saturday, new TimeOnly(9, 0), new TimeOnly(13, 0)));

        var internalCentral = new Department { Name = "Internal Medicine" };
        var radiology = new Department { Name = "Radiology" };
        central.AddDepartment(internalCentral);
        central.AddDepartment(radiology);

        var internalNorth = new Department { Name = "Internal Medicine" };
        var laboratory = new Department { Name = "Laboratory" };
        north.AddDepartment(internalNorth);
        north.AddDepartment(laboratory);

        var examination = new MedicalService("General Examination", ServiceCategory.GeneralExamination, TimeSpan.FromMinutes(20), 500m);
        var xray = new MedicalService("X-Ray", ServiceCategory.Radiology, TimeSpan.FromMinutes(15), 750m);
        var ultrasound = new MedicalService("Ultrasound", ServiceCategory.Radiology, TimeSpan.FromMinutes(30), 1200m);
        var bloodTest = new MedicalService("Blood Test", ServiceCategory.Laboratory, TimeSpan.FromMinutes(10), 300m);
        foreach (var service in new[] { examination, xray, ultrasound, bloodTest })
            institution.AddService(service);

        var doctors = new[]
        {
            CreateDoctor("Ebru", "Yazıcı", "ebru.yazici@hospital.com", "Internal Medicine", central, internalCentral, examination),
            CreateDoctor("Mehmet", "Kaya", "mehmet.kaya@hospital.com", "Radiology", central, radiology, xray, ultrasound),
            CreateDoctor("Zeynep", "Demir", "zeynep.demir@hospital.com", "Internal Medicine", north, internalNorth, examination, bloodTest)
        };

        var admin = new Admin
        {
            FirstName = "System",
            LastName = "Admin",
            Email = "admin@hospital.com",
            PhoneNumber = "0264 000 00 00"
        };
        admin.SetPassword("Admin123!");

        context.Institutions.Add(institution);
        context.Doctors.AddRange(doctors);
        context.Admins.Add(admin);

        if (!context.Users.Any(u => u.Email == "patient@test.com"))
        {
            var patient = new Patient
            {
                FirstName = "Test",
                LastName = "Patient",
                Email = "patient@test.com",
                PhoneNumber = "0555 000 00 00",
                NationalId = "12345678901",
                DateOfBirth = new DateTime(2000, 1, 1)
            };
            patient.SetPassword("Patient123!");
            context.Patients.Add(patient);
        }

        context.SaveChanges();
    }

    private static Doctor CreateDoctor(string firstName, string lastName, string email, string specialty,
        Branch branch, Department department, params MedicalService[] services)
    {
        var doctor = new Doctor
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = "0264 000 10 00",
            Specialty = specialty,
            Branch = branch,
            Department = department
        };
        doctor.SetPassword("Doctor123!");

        foreach (var service in services)
            doctor.AddService(service);

        for (var i = 1; i <= 14; i++)
        {
            var day = DateTime.Today.AddDays(i);
            if (day.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                continue;

            doctor.AddAvailability(new AvailabilitySlot(day.AddHours(9), day.AddHours(12)));
            doctor.AddAvailability(new AvailabilitySlot(day.AddHours(13), day.AddHours(17)));
        }

        return doctor;
    }
}