# Hospital Appointment System

A web-based hospital/clinic appointment system developed as an Object-Oriented Programming course project. The system supports both single clinics and multi-branch healthcare institutions.

## Features

- [x] Hospital/clinic and branch structure with working hours and departments
- [x] Medical services with duration and price
- [x] Doctors linked to branches, departments and the services they offer
- [x] Doctor availability slots
- [x] Appointment booking with conflict checking (branch hours, doctor availability, doctor and patient overlaps)
- [x] Appointment approval workflow (Pending → Approved / Rejected → Completed, or Cancelled)
- [x] User registration and login
- [ ] Role-based pages: Patient, Doctor, Admin

## Technologies

- C# / ASP.NET Core MVC (.NET 8)
- Entity Framework Core 8
- PostgreSQL

## Project Structure

```
HospitalAppointmentSystem.Web/
├── Models/         Domain classes (User hierarchy, Institution, Appointment, ...)
├── Services/       Business logic (AppointmentService with conflict checking)
├── Repositories/   Data access abstraction and EF Core implementation
├── Data/           AppDbContext (database configuration)
├── Migrations/     Database schema history
├── Controllers/    MVC controllers
└── Views/          Razor pages
```

## How to Run

1. Install the [.NET 8 SDK](https://dotnet.microsoft.com/download) and [PostgreSQL](https://www.postgresql.org/download/).
2. Clone the repository.
3. Install the EF Core command-line tool:
```bash
   dotnet tool install --global dotnet-ef
```
4. Set your database connection (inside the `HospitalAppointmentSystem.Web` folder):
```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=hospital_appointment;Username=postgres;Password=YOUR_PASSWORD"
```
   Adjust the port and password to match your PostgreSQL installation.
5. Create the database:
```bash
   dotnet ef database update
```
6. Run the application:
```bash
   dotnet run
```