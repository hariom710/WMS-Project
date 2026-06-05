namespace WMS.Domain.Interfaces
{
    public interface IPdfReportService
    {
        Task<byte[]> ExportEmployeesPdfAsync(string? search, string? status);
        Task<byte[]> ExportAttendancePdfAsync(int? empId, int? month, int? year);
        Task<byte[]> ExportLeavesPdfAsync(string? status);
        Task<byte[]> ExportProjectsPdfAsync(string? status);
        Task<byte[]> ExportDashboardPdfAsync();
    }
}
