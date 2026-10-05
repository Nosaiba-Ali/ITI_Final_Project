using Final_Project.Data;
using Final_Project.Enums;
using Final_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Final_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CustomerController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: api/Customer/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized(new { message = "User is not authenticated." });

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound(new { message = "Customer not found." });

            return Ok(new
            {
                id = user.Id,
                fullName = user.FullName,
                email = user.Email,
                createdAt = user.CreatedAt
            });
        }

        // GET: api/Customer/products
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    description = p.Description,
                    price = p.Price,
                    stockQuantity = p.StockQuantity,
                    imageUrl = p.ImageUrl,
                    category = p.Category != null ? p.Category.Name : null
                })
                .ToListAsync();

            return Ok(products);
        }

        // GET: api/Customer/products/search?keyword=phone
        [HttpGet("products/search")]
        public async Task<IActionResult> SearchProducts(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Search keyword is required." });

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p =>
                    p.Name.Contains(keyword) ||
                    p.Description.Contains(keyword))
                .Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    description = p.Description,
                    price = p.Price,
                    stockQuantity = p.StockQuantity,
                    imageUrl = p.ImageUrl,
                    category = p.Category != null ? p.Category.Name : null
                })
                .ToListAsync();

            return Ok(products);
        }

        // GET: api/Customer/products/{id}
        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProductDetails(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound(new { message = "Product not found." });

            return Ok(new
            {
                id = product.Id,
                name = product.Name,
                description = product.Description,
                price = product.Price,
                stockQuantity = product.StockQuantity,
                imageUrl = product.ImageUrl,
                category = product.Category != null ? product.Category.Name : null
            });
        }

        // POST: api/Customer/wishlist/{productId}
        [HttpPost("wishlist/{productId}")]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized(new { message = "User is not authenticated." });

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                return NotFound(new { message = "Product not found." });

            var existingItem = await _context.WishlistItems
                .FirstOrDefaultAsync(w =>
                    w.CustomerId == userId &&
                    w.ProductId == productId);

            if (existingItem != null)
                return BadRequest(new { message = "Product is already in wishlist." });

            var wishlistItem = new WishlistItem
            {
                CustomerId = userId,
                ProductId = productId
            };

            _context.WishlistItems.Add(wishlistItem);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Product added to wishlist."
            });
        }

        // DELETE: api/Customer/wishlist/{productId}
        [HttpDelete("wishlist/{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized(new { message = "User is not authenticated." });

            var wishlistItem = await _context.WishlistItems
                .FirstOrDefaultAsync(w =>
                    w.CustomerId == userId &&
                    w.ProductId == productId);

            if (wishlistItem == null)
                return NotFound(new
                {
                    message = "Product is not in your wishlist."
                });

            _context.WishlistItems.Remove(wishlistItem);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Product removed from wishlist."
            });
        }

        // GET: api/Customer/orders
        [HttpGet("orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized(new { message = "User is not authenticated." });

            var orders = await _context.Orders
                .Where(o => o.CustomerId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    id = o.Id,
                    orderDate = o.OrderDate,
                    status = o.Status.ToString(),
                    totalPrice = o.TotalPrice
                })
                .ToListAsync();

            return Ok(orders);
        }

        // GET: api/Customer/orders/{id}
        [HttpGet("orders/{id}")]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized(new { message = "User is not authenticated." });

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o =>
                    o.Id == id &&
                    o.CustomerId == userId);

            if (order == null)
                return NotFound(new
                {
                    message = "Order not found."
                });

            return Ok(new
            {
                id = order.Id,
                orderDate = order.OrderDate,
                status = order.Status.ToString(),
                totalPrice = order.TotalPrice,
                items = order.OrderItems.Select(oi => new
                {
                    productId = oi.ProductId,
                    productName = oi.Product != null ? oi.Product.Name : null,
                    quantity = oi.Quantity,
                    unitPrice = oi.UnitPrice,
                    sellerId = oi.SellerId
                })
            });
        }

        // PUT: api/Customer/orders/{id}/cancel
        [HttpPut("orders/{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized(new { message = "User is not authenticated." });

            var order = await _context.Orders
                .FirstOrDefaultAsync(o =>
                    o.Id == id &&
                    o.CustomerId == userId);

            if (order == null)
                return NotFound(new
                {
                    message = "Order not found."
                });

            if (order.Status == OrderStatus.Cancelled)
                return BadRequest(new
                {
                    message = "Order is already cancelled."
                });

            if (order.Status != OrderStatus.Pending)
                return BadRequest(new
                {
                    message = "Only pending orders can be cancelled."
                });

            order.Status = OrderStatus.Cancelled;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Order cancelled successfully."
            });
        }
    }
}

