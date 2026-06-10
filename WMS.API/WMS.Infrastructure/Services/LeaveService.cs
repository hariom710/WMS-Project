using WMS.Domain.Interfaces;
using WMS.Domain.Models;

namespace WMS.Infrastructure.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _leaveRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IActivityLogService _activityLog;

        public LeaveService(ILeaveRepository leaveRepo, IEmployeeRepository employeeRepo, IActivityLogService activityLog)
        {
            _leaveRepo = leaveRepo;
            _employeeRepo = employeeRepo;
            _activityLog = activityLog;
        }

        public async Task<PagedResult<Leave>> GetAllAsync(string? search, string? status, string? sortBy, string? sortDirection, int page, int pageSize) =>
            await _leaveRepo.GetAllAsync(search, status, sortBy, sortDirection, page, pageSize);

        public async Task<PagedResult<Leave>> GetDeletedAsync(string? search, string? sortBy, string? sortDirection, int page, int pageSize) =>
            await _leaveRepo.GetDeletedAsync(search, sortBy, sortDirection, page, pageSize);

        public async Task<IEnumerable<Leave>> GetPendingAsync() =>
            await _leaveRepo.GetPendingAsync();

        public async Task<(bool success, string message)> CreateAsync(Leave leave, string userEmail)
        {
            if (leave.Reason != null) leave.Reason = leave.Reason.Trim();
            if (string.IsNullOrWhiteSpace(leave.Reason) || leave.Reason.Length < 10)
                return (false, "Reason must be at least 10 characters.");
            if (leave.FromDate >= leave.ToDate)
                return (false, "ToDate must be after FromDate.");

            var employee = await _employeeRepo.GetByEmailAsync(userEmail);

            if (employee == null && leave.EmpId > 0)
            {
                var empById = await _employeeRepo.GetByIdAsync(leave.EmpId);
                if (empById != null) employee = empById;
            }

            if (employee == null) return (false, "Employee not found.");

            if (await _leaveRepo.HasOverlappingLeaveAsync(employee.EmployeeId, leave.FromDate, leave.ToDate))
                return (false, "Leave request overlaps with existing leave.");

            leave.EmpId = employee.EmployeeId;
            leave.CreatedBy = userEmail;
            leave.CreatedDate = DateTime.UtcNow;
            leave.Status = "Pending";
            await _leaveRepo.AddAsync(leave);

            await _activityLog.LogAsync("Leave", leave.LeaveId, "Create",
                $"Submitted leave request ({leave.FromDate:yyyy-MM-dd} to {leave.ToDate:yyyy-MM-dd})", userEmail, null, null);

            return (true, "Leave request submitted successfully!");
        }

        public async Task<(bool success, string message)> ApproveAsync(int id, string userEmail)
        {
            var leave = await _leaveRepo.GetByIdAsync(id);
            if (leave == null) return (false, "Leave not found.");

            var manager = await _employeeRepo.GetByEmailAsync(userEmail);
            leave.Status = "Approved";
            leave.ApprovedBy = manager?.EmployeeId ?? 1;
            leave.ApprovedOn = DateTime.Now;
            leave.ModifiedBy = userEmail;
            leave.ModifiedDate = DateTime.UtcNow;
            await _leaveRepo.UpdateAsync(leave);

            await _activityLog.LogAsync("Leave", id, "Approve",
                $"Approved leave for employee ID {leave.EmpId}", userEmail, null, null);

            return (true, "Leave Approved!");
        }

        public async Task<(bool success, string message)> RejectAsync(int id, string? reason, string userEmail)
        {
            var leave = await _leaveRepo.GetByIdAsync(id);
            if (leave == null) return (false, "Leave not found.");

            var manager = await _employeeRepo.GetByEmailAsync(userEmail);
            leave.Status = "Rejected";
            leave.ApprovedBy = manager?.EmployeeId ?? 1;
            leave.ApprovedOn = DateTime.Now;
            leave.ModifiedBy = userEmail;
            leave.ModifiedDate = DateTime.UtcNow;
            await _leaveRepo.UpdateAsync(leave);

            var desc = string.IsNullOrEmpty(reason) ? $"Rejected leave for employee ID {leave.EmpId}" : $"Rejected leave for employee ID {leave.EmpId}. Reason: {reason}";
            await _activityLog.LogAsync("Leave", id, "Reject", desc, userEmail, null, null);

            var msg = string.IsNullOrEmpty(reason) ? "Leave Rejected!" : $"Leave Rejected. Reason: {reason}";
            return (true, msg);
        }

        public async Task<(bool success, string message)> SoftDeleteAsync(int id, string userEmail)
        {
            var leave = await _leaveRepo.GetByIdAsync(id);
            if (leave == null) return (false, "Leave not found.");
            await _leaveRepo.SoftDeleteAsync(leave, userEmail);

            await _activityLog.LogAsync("Leave", id, "Delete",
                $"Deleted leave request for employee ID {leave.EmpId}", userEmail, null, null);

            return (true, "Leave deleted successfully!");
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var deleted = await _leaveRepo.GetDeletedAsync(null, null, null, 1, int.MaxValue);
            var leave = deleted.Items.FirstOrDefault(l => l.LeaveId == id);
            if (leave == null) return false;
            await _leaveRepo.RestoreAsync(leave);

            await _activityLog.LogAsync("Leave", id, "Restore",
                $"Restored leave request for employee ID {leave.EmpId}", null, null, null);

            return true;
        }
    }
}
