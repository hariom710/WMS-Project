using WMS.Domain.Models;

namespace WMS.Domain.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync();
    }
}
