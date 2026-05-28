using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLeaveTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AppliedOn",
                table: "Leaves",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ApprovedBy",
                table: "Leaves",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedOn",
                table: "Leaves",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leaves_EmpId",
                table: "Leaves",
                column: "EmpId");

            migrationBuilder.AddForeignKey(
                name: "FK_Leaves_Employees_EmpId",
                table: "Leaves",
                column: "EmpId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leaves_Employees_EmpId",
                table: "Leaves");

            migrationBuilder.DropIndex(
                name: "IX_Leaves_EmpId",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "AppliedOn",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "ApprovedOn",
                table: "Leaves");
        }
    }
}
