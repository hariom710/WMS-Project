using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.API.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeleteAndAuditTrail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ProjectAllocations");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "ProjectAllocations",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "ProjectAllocations",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "AppliedOn",
                table: "Leaves",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "Employees",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Employees",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Departments",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "AuditLogs",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Announcements",
                newName: "CreatedDate");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Projects",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Projects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProjectName",
                table: "Projects",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Projects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Projects",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Projects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Projects",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Projects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Projects",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "ProjectAllocations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ProjectAllocations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "ProjectAllocations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProjectAllocations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "ProjectAllocations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "ProjectAllocations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Leaves",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Leaves",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LeaveType",
                table: "Leaves",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Leaves",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Leaves",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Leaves",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Leaves",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Leaves",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Leaves",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Employees",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1)",
                oldMaxLength: 1);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Employees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Departments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Departments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Departments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Departments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Departments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Departments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Departments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClientPhoneNumber",
                table: "Clients",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClientLocation",
                table: "Clients",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClientAddress",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Clients",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Clients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Clients",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Clients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Clients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Clients",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Clients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EntityName",
                table: "AuditLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AuditLogs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "AuditLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserRole",
                table: "AuditLogs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "AuditLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Announcements",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Announcements",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Announcements",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByEmployeeId",
                table: "Announcements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Announcements",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Announcements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Announcements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Announcements",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Announcements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ClientId",
                table: "Projects",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_IsDeleted",
                table: "Projects",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Status",
                table: "Projects",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Status_ClientId",
                table: "Projects",
                columns: new[] { "Status", "ClientId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAllocations_EmpId_ProjectId",
                table: "ProjectAllocations",
                columns: new[] { "EmpId", "ProjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAllocations_IsDeleted",
                table: "ProjectAllocations",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAllocations_ProjectId_Status",
                table: "ProjectAllocations",
                columns: new[] { "ProjectId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Leaves_EmpId_Status",
                table: "Leaves",
                columns: new[] { "EmpId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Leaves_FromDate",
                table: "Leaves",
                column: "FromDate");

            migrationBuilder.CreateIndex(
                name: "IX_Leaves_IsDeleted",
                table: "Leaves",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Leaves_Status",
                table: "Leaves",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_IsDeleted",
                table: "Employees",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_LastName",
                table: "Employees",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PhoneNumber",
                table: "Employees",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Status",
                table: "Employees",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_IsDeleted",
                table: "Departments",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ClientName",
                table: "Clients",
                column: "ClientName");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_IsDeleted",
                table: "Clients",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Action",
                table: "AuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityName",
                table: "AuditLogs",
                column: "EntityName");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityName_Action",
                table: "AuditLogs",
                columns: new[] { "EntityName", "Action" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_RecordId",
                table: "AuditLogs",
                column: "RecordId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Username",
                table: "AuditLogs",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Username_Timestamp",
                table: "AuditLogs",
                columns: new[] { "Username", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_AttendanceDate",
                table: "Attendances",
                column: "AttendanceDate");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_EmpId_AttendanceDate",
                table: "Attendances",
                columns: new[] { "EmpId", "AttendanceDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_CreatedByEmployeeId",
                table: "Announcements",
                column: "CreatedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_IsActive",
                table: "Announcements",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_IsDeleted",
                table: "Announcements",
                column: "IsDeleted");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Employees_CreatedByEmployeeId",
                table: "Announcements",
                column: "CreatedByEmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Clients_ClientId",
                table: "Projects",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Employees_CreatedByEmployeeId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Clients_ClientId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ClientId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_IsDeleted",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_Status",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_Status_ClientId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_ProjectAllocations_EmpId_ProjectId",
                table: "ProjectAllocations");

            migrationBuilder.DropIndex(
                name: "IX_ProjectAllocations_IsDeleted",
                table: "ProjectAllocations");

            migrationBuilder.DropIndex(
                name: "IX_ProjectAllocations_ProjectId_Status",
                table: "ProjectAllocations");

            migrationBuilder.DropIndex(
                name: "IX_Leaves_EmpId_Status",
                table: "Leaves");

            migrationBuilder.DropIndex(
                name: "IX_Leaves_FromDate",
                table: "Leaves");

            migrationBuilder.DropIndex(
                name: "IX_Leaves_IsDeleted",
                table: "Leaves");

            migrationBuilder.DropIndex(
                name: "IX_Leaves_Status",
                table: "Leaves");

            migrationBuilder.DropIndex(
                name: "IX_Employees_IsDeleted",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_LastName",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_PhoneNumber",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Status",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Departments_IsDeleted",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Clients_ClientName",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_IsDeleted",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_Action",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_EntityName",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_EntityName_Action",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_RecordId",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_Username",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_Username_Timestamp",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_AttendanceDate",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_EmpId_AttendanceDate",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_CreatedByEmployeeId",
                table: "Announcements");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_IsActive",
                table: "Announcements");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_IsDeleted",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ProjectAllocations");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "ProjectAllocations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProjectAllocations");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "ProjectAllocations");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "ProjectAllocations");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Leaves");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "UserRole",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "CreatedByEmployeeId",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Announcements");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "ProjectAllocations",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "ProjectAllocations",
                newName: "CreateDate");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Leaves",
                newName: "AppliedOn");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "Employees",
                newName: "UpdatedOn");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Employees",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Departments",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "AuditLogs",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Announcements",
                newName: "CreatedOn");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Projects",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ProjectName",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Projects",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "ProjectAllocations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ProjectAllocations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Leaves",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Leaves",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "LeaveType",
                table: "Leaves",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Employees",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Employees",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1)",
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Departments",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "ClientPhoneNumber",
                table: "Clients",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "ClientLocation",
                table: "Clients",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "ClientAddress",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EntityName",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "AuditLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Announcements",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Announcements",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "Announcements",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
