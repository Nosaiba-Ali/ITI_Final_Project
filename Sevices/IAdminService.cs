using Final_Project.ViewModels.Admin;

namespace Final_Project.Services
{
    public interface IAdminService
    {
        Task<AdminDashboardDto> GetAdminDashboardStatsAsync();

        Task<SellerDashboardDto> GetSellerDashboardStatsAsync(string sellerId);
    }
}