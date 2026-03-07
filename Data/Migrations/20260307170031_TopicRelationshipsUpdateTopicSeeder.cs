using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace szpont.Data.Migrations
{
    /// <inheritdoc />
    public partial class TopicRelationshipsUpdateTopicSeeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "Topics",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DziekanId",
                table: "Topics",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KierownikId",
                table: "Topics",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromotorId",
                table: "Topics",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Topics",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Topics",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedDate",
                table: "Topics",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Topics_DziekanId",
                table: "Topics",
                column: "DziekanId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_KierownikId",
                table: "Topics",
                column: "KierownikId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_PromotorId",
                table: "Topics",
                column: "PromotorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_AspNetUsers_DziekanId",
                table: "Topics",
                column: "DziekanId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_AspNetUsers_KierownikId",
                table: "Topics",
                column: "KierownikId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_AspNetUsers_PromotorId",
                table: "Topics",
                column: "PromotorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_AspNetUsers_DziekanId",
                table: "Topics");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_AspNetUsers_KierownikId",
                table: "Topics");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_AspNetUsers_PromotorId",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_DziekanId",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_KierownikId",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_PromotorId",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "DziekanId",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "KierownikId",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "PromotorId",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "SubmittedDate",
                table: "Topics");
        }
    }
}
