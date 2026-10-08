using Final_Project.Data;
using Final_Project.Enums;
using Final_Project.Models;
using Final_Project.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;

namespace Final_Project.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<AdminDashboardDto> GetAdminDashboardStatsAsync()
        {
            return new AdminDashboardDto
            {
                TotalCustomers = await _context.Users.CountAsync(),
                TotalProducts = await _context.Products.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                PendingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending)
            };
        }
        public async Task<SellerDashboardDto> GetSellerDashboardStatsAsync(string sellerId)
        {
            var totalProducts = await _context.Products.CountAsync(p => p.SellerId == sellerId);

            // جلب المنتجات التابعة للبائع
            var sellerProductIds = await _context.Products
                .Where(p => p.SellerId == sellerId)
                .Select(p => p.Id)
                .ToListAsync();
            var totalOrders = await _context.Orders
                .Where(o => o.OrderItems.Any(oi => sellerProductIds.Contains(oi.ProductId)))
                .CountAsync();

            var totalSales = await _context.Orders
                .Where(o => o.OrderItems.Any(oi => sellerProductIds.Contains(oi.ProductId)))
                .SelectMany(o => o.OrderItems)
                .Where(oi => sellerProductIds.Contains(oi.ProductId))
                .SumAsync(oi => oi.UnitPrice * oi.Quantity);

            return new SellerDashboardDto
            {
                TotalProducts = totalProducts,
                TotalOrders = totalOrders,
                TotalSales = totalSales
            };
        }
    }
}