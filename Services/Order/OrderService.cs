using Final_Project.Data;
using Final_Project.DTOs.Order;
using Final_Project.Enums;
using Final_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_Project.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OrderResponseDto> CheckoutAsync(string userId, CreateOrderDto dto)
        {
            var cartItems = await _context.CartItems
                .Include(i => i.Product)
                .Where(i => i.CustomerId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                throw new InvalidOperationException("The cart is empty, cannot proceed with checkout.");

            var order = new Final_Project.Models.Order
            {
                CustomerId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalPrice = cartItems.Sum(i => (i.Product?.Price ?? 0) * i.Quantity),
                OrderItems = cartItems.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    SellerId = i.Product?.SellerId ?? string.Empty,
                    UnitPrice = i.Product?.Price ?? 0,
                    Quantity = i.Quantity
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return MapToResponseDto(order, $"{dto.ShippingAddress}, {dto.City} (Phone: {dto.Phone})");
        }

        public async Task<List<OrderResponseDto>> GetCustomerOrdersAsync(string userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.CustomerId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(o => MapToResponseDto(o, string.Empty)).ToList();
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(string userId, int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.CustomerId == userId && o.Id == orderId);

            return order == null ? null : MapToResponseDto(order, string.Empty);
        }

        private static OrderResponseDto MapToResponseDto(Final_Project.Models.Order order, string address)
        {
            return new OrderResponseDto
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString(),
                ShippingAddress = address,
                TotalAmount = order.TotalPrice,
                Items = order.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    OrderItemId = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? string.Empty,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity
                }).ToList()
            };
        }
    }
}