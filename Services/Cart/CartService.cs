using Final_Project.Data;
using Final_Project.DTOs.Cart;
using Final_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_Project.Services.Cart
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CartResponseDto> GetCartByUserIdAsync(string userId)
        {
            var items = await _context.CartItems
                .Include(i => i.Product)
                .Where(i => i.CustomerId == userId)
                .ToListAsync();

            return MapToResponseDto(items);
        }

        public async Task<CartResponseDto> AddToCartAsync(string userId, AddToCartDto dto)
        {
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(i => i.CustomerId == userId && i.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    CustomerId = userId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return await GetCartByUserIdAsync(userId);
        }

        public async Task<CartResponseDto> UpdateCartItemQuantityAsync(string userId, UpdateCartItemDto dto)
        {
            var item = await _context.CartItems
                .FirstOrDefaultAsync(i => i.CustomerId == userId && i.Id == dto.CartItemId);

            if (item != null)
            {
                if (dto.Quantity <= 0)
                {
                    _context.CartItems.Remove(item);
                }
                else
                {
                    item.Quantity = dto.Quantity;
                }

                await _context.SaveChangesAsync();
            }

            return await GetCartByUserIdAsync(userId);
        }

        public async Task<CartResponseDto> RemoveFromCartAsync(string userId, int cartItemId)
        {
            var item = await _context.CartItems
                .FirstOrDefaultAsync(i => i.CustomerId == userId && i.Id == cartItemId);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return await GetCartByUserIdAsync(userId);
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            var items = await _context.CartItems
                .Where(i => i.CustomerId == userId)
                .ToListAsync();

            if (!items.Any())
                return false;

            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
            return true;
        }

        private static CartResponseDto MapToResponseDto(List<CartItem> items)
        {
            return new CartResponseDto
            {
                CartId = 1, 
                Items = items.Select(i => new CartItemResponseDto
                {
                    CartItemId = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? string.Empty,
                    ProductImageUrl = i.Product?.ImageUrl,
                    UnitPrice = i.Product?.Price ?? 0,
                    Quantity = i.Quantity
                }).ToList()
            };
        }
    }
}