using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using szpont.Data;

#nullable disable

namespace szpont.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260420120000_AddReservationStatusToTopics")]
    public partial class AddReservationStatusToTopics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReservationStatus",
                table: "Topics",
                type: "INTEGER",
                nullable: true);

            // Istniejące przypisania studentów traktujemy jak już zaakceptowane (Accepted = 1).
            migrationBuilder.Sql(
                "UPDATE Topics SET ReservationStatus = 1 WHERE StudentId IS NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReservationStatus",
                table: "Topics");
        }
    }
}
