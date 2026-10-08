using Final_Project.DTOs.Order;

namespace Final_Project.Services.Order
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CheckoutAsync(string userId, CreateOrderDto dto);
        Task<List<OrderResponseDto>> GetCustomerOrdersAsync(string userId);
        Task<OrderResponseDto?> GetOrderByIdAsync(string userId, int orderId);
    }
}