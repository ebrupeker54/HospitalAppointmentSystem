using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalAppointmentSystem.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddSecretary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SecretaryDoctors",
                columns: table => new
                {
                    DoctorsId = table.Column<int>(type: "integer", nullable: false),
                    SecretariesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecretaryDoctors", x => new { x.DoctorsId, x.SecretariesId });
                    table.ForeignKey(
                        name: "FK_SecretaryDoctors_Users_DoctorsId",
                        column: x => x.DoctorsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SecretaryDoctors_Users_SecretariesId",
                        column: x => x.SecretariesId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SecretaryDoctors_SecretariesId",
                table: "SecretaryDoctors",
                column: "SecretariesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SecretaryDoctors");
        }
    }
}
