using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalAppointmentSystem.Web.Migrations
{
    /// <inheritdoc />
    public partial class ProfessionalHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_DoctorId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_AvailabilitySlots_Users_DoctorId",
                table: "AvailabilitySlots");

            migrationBuilder.DropTable(
                name: "DoctorMedicalService");

            migrationBuilder.RenameColumn(
                name: "DoctorId",
                table: "AvailabilitySlots",
                newName: "ProfessionalId");

            migrationBuilder.RenameIndex(
                name: "IX_AvailabilitySlots_DoctorId",
                table: "AvailabilitySlots",
                newName: "IX_AvailabilitySlots_ProfessionalId");

            migrationBuilder.RenameColumn(
                name: "DoctorId",
                table: "Appointments",
                newName: "ProfessionalId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_DoctorId",
                table: "Appointments",
                newName: "IX_Appointments_ProfessionalId");

            migrationBuilder.AddColumn<string>(
                name: "UserType",
                table: "Users",
                type: "character varying(34)",
                maxLength: 34,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "HealthcareProfessionalMedicalService",
                columns: table => new
                {
                    ProfessionalsId = table.Column<int>(type: "integer", nullable: false),
                    ServicesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthcareProfessionalMedicalService", x => new { x.ProfessionalsId, x.ServicesId });
                    table.ForeignKey(
                        name: "FK_HealthcareProfessionalMedicalService_MedicalServices_Servic~",
                        column: x => x.ServicesId,
                        principalTable: "MedicalServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HealthcareProfessionalMedicalService_Users_ProfessionalsId",
                        column: x => x.ProfessionalsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HealthcareProfessionalMedicalService_ServicesId",
                table: "HealthcareProfessionalMedicalService",
                column: "ServicesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_ProfessionalId",
                table: "Appointments",
                column: "ProfessionalId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AvailabilitySlots_Users_ProfessionalId",
                table: "AvailabilitySlots",
                column: "ProfessionalId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_ProfessionalId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_AvailabilitySlots_Users_ProfessionalId",
                table: "AvailabilitySlots");

            migrationBuilder.DropTable(
                name: "HealthcareProfessionalMedicalService");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "ProfessionalId",
                table: "AvailabilitySlots",
                newName: "DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_AvailabilitySlots_ProfessionalId",
                table: "AvailabilitySlots",
                newName: "IX_AvailabilitySlots_DoctorId");

            migrationBuilder.RenameColumn(
                name: "ProfessionalId",
                table: "Appointments",
                newName: "DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_ProfessionalId",
                table: "Appointments",
                newName: "IX_Appointments_DoctorId");

            migrationBuilder.CreateTable(
                name: "DoctorMedicalService",
                columns: table => new
                {
                    DoctorsId = table.Column<int>(type: "integer", nullable: false),
                    ServicesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorMedicalService", x => new { x.DoctorsId, x.ServicesId });
                    table.ForeignKey(
                        name: "FK_DoctorMedicalService_MedicalServices_ServicesId",
                        column: x => x.ServicesId,
                        principalTable: "MedicalServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DoctorMedicalService_Users_DoctorsId",
                        column: x => x.DoctorsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorMedicalService_ServicesId",
                table: "DoctorMedicalService",
                column: "ServicesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_DoctorId",
                table: "Appointments",
                column: "DoctorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AvailabilitySlots_Users_DoctorId",
                table: "AvailabilitySlots",
                column: "DoctorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
