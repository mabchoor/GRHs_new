using Microsoft.EntityFrameworkCore.Migrations;

namespace GRHs.Data.UserSession
{
    public partial class seconde : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalendarHolidays");

            migrationBuilder.DropTable(
                name: "Calendars");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calendars",
                columns: table => new
                {
                    CalendarID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendars", x => x.CalendarID);
                });

            migrationBuilder.CreateTable(
                name: "CalendarHolidays",
                columns: table => new
                {
                    CalendarID = table.Column<int>(type: "int", nullable: false),
                    HolidayID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarHolidays", x => new { x.CalendarID, x.HolidayID });
                    table.ForeignKey(
                        name: "FK_CalendarHolidays_Calendars_CalendarID",
                        column: x => x.CalendarID,
                        principalTable: "Calendars",
                        principalColumn: "CalendarID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CalendarHolidays_Holidays_HolidayID",
                        column: x => x.HolidayID,
                        principalTable: "Holidays",
                        principalColumn: "HolidayID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarHolidays_HolidayID",
                table: "CalendarHolidays",
                column: "HolidayID");
        }
    }
}
