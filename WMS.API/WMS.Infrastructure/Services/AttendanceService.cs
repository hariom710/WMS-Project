using WMS.Domain.Interfaces;
using WMS.Domain.Models;

namespace WMS.Infrastructure.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IActivityLogService _activityLog;

        public AttendanceService(IAttendanceRepository attendanceRepo, IEmployeeRepository employeeRepo, IActivityLogService activityLog)
        {
            _attendanceRepo = attendanceRepo;
            _employeeRepo = employeeRepo;
            _activityLog = activityLog;
        }

        public async Task<PagedResult<Attendance>> GetAllAsync(string? search, int? empId, int? month, int? year, string? sortBy, string? sortDirection, int page, int pageSize) =>
            await _attendanceRepo.GetAllAsync(search, empId, month, year, sortBy, sortDirection, page, pageSize);

        public async Task<(bool success, string message)> CreateAsync(Attendance attendance)
        {
            if (!await _employeeRepo.ExistsAsync(attendance.EmpId))
                return (false, "Employee does not exist.");
            if (await _attendanceRepo.HasCheckedInTodayAsync(attendance.EmpId))
                return (false, "Employee has already checked in today.");

            attendance.AttendanceDate = DateTime.Today;
            attendance.CheckIn = DateTime.Now;
            await _attendanceRepo.AddAsync(attendance);
            return (true, "Clocked in successfully!");
        }

        public async Task<(bool success, string message)> UpdateAsync(int id, Attendance attendance)
        {
            await _attendanceRepo.UpdateAsync(attendance);
            return (true, "Attendance updated!");
        }

        public async Task<(bool success, string message)> CheckInAsync(string workMode, string userEmail)
        {
            var employee = await _employeeRepo.GetByEmailAsync(userEmail);
            if (employee == null) return (false, "Employee not found.");
            if (await _attendanceRepo.HasCheckedInTodayAsync(employee.EmployeeId))
                return (false, "You have already checked in today!");

            var attendance = new Attendance
            {
                EmpId = employee.EmployeeId,
                CheckIn = DateTime.Now,
                AttendanceDate = DateTime.Today,
                WorkMode = workMode ?? "Office"
            };
            await _attendanceRepo.AddAsync(attendance);

            await _activityLog.LogAsync("Attendance", attendance.AttendanceId, "CheckIn",
                $"Checked in ({workMode ?? "Office"})", userEmail, null, null);

            return (true, "Checked in successfully!");
        }

        public async Task<(bool success, string message)> CheckOutAsync(string userEmail)
        {
            var employee = await _employeeRepo.GetByEmailAsync(userEmail);
            if (employee == null) return (false, "Employee not found.");

            var attendance = await _attendanceRepo.GetTodayByEmployeeAsync(employee.EmployeeId);
            if (attendance == null) return (false, "No check-in record found for today.");
            if (attendance.CheckOut != null) return (false, "You have already checked out today!");

            attendance.CheckOut = DateTime.Now;
            var duration = attendance.CheckOut.Value - attendance.CheckIn;
            attendance.TotalHours = (float)Math.Round(duration.TotalHours, 2);
            await _attendanceRepo.UpdateAsync(attendance);

            await _activityLog.LogAsync("Attendance", attendance.AttendanceId, "CheckOut",
                $"Checked out. Total hours: {attendance.TotalHours}", userEmail, null, null);

            return (true, "Checked out successfully!");
        }
    }
}
