using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDbCheckConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Meetings_TripId",
                table: "Meetings");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "TeamPlanEntries",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Trips_DateOrder",
                table: "Trips",
                sql: "\"EndDate\" >= \"StartDate\"");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TeamPlanEntries_ApprovalOnlyVacation",
                table: "TeamPlanEntries",
                sql: "\"ApprovalStatus\" = '' OR \"Type\" = 'Vacation'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TeamPlanEntries_DateOrder",
                table: "TeamPlanEntries",
                sql: "\"ToDate\" >= \"FromDate\"");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_TripId_DisplayOrder",
                table: "Meetings",
                columns: new[] { "TripId", "DisplayOrder" },
                unique: true,
                filter: "\"IsActive\" = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Trips_DateOrder",
                table: "Trips");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TeamPlanEntries_ApprovalOnlyVacation",
                table: "TeamPlanEntries");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TeamPlanEntries_DateOrder",
                table: "TeamPlanEntries");

            migrationBuilder.DropIndex(
                name: "IX_Meetings_TripId_DisplayOrder",
                table: "Meetings");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "TeamPlanEntries",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_TripId",
                table: "Meetings",
                column: "TripId");
        }
    }
}
