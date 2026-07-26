using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class CreateFlights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Aircraft",
                table: "Flights",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Flights",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Flights_UserId",
                table: "Flights",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Users_UserId",
                table: "Flights",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Users_UserId",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_UserId",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "Aircraft",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Flights");
        }
    }
}
