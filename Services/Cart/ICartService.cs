using Final_Project.DTOs.Cart;

namespace Final_Project.Services.Cart
{
	public interface ICartService
	{
		Task<CartResponseDto> GetCartByUserIdAsync(string userId);
		Task<CartResponseDto> AddToCartAsync(string userId, AddToCartDto dto);
		Task<CartResponseDto> UpdateCartItemQuantityAsync(string userId, UpdateCartItemDto dto);
		Task<CartResponseDto> RemoveFromCartAsync(string userId, int cartItemId);
		Task<bool> ClearCartAsync(string userId);
	}
}