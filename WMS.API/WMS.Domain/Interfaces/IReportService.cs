namespace WMS.Domain.Interfaces
{
    public interface IReportService
    {
        Task<byte[]> ExportEmployeesToExcelAsync(string? search, string? status);
        Task<byte[]> ExportAttendanceToExcelAsync(int? empId, int? month, int? year);
        Task<byte[]> ExportLeavesToExcelAsync(string? status);
        Task<byte[]> ExportProjectsToExcelAsync(string? status);
        Task<byte[]> ExportClientsToExcelAsync();
    }
}
