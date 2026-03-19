using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace szpont.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentReservationToTopics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReservationDate",
                table: "Topics",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentId",
                table: "Topics",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Topics_StudentId",
                table: "Topics",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_AspNetUsers_StudentId",
                table: "Topics",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_AspNetUsers_StudentId",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_StudentId",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "ReservationDate",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Topics");
        }
    }
}
