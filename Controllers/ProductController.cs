using Final_Project.Models;
using Final_Project.Sevices.Interfaces;
using Final_Project.ViewModels.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Final_Project.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductController(IProductService productService, UserManager<ApplicationUser> userManager)
        {
            _productService = productService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string? searchTerm, int? categoryId, string? sortOrder)
        {
            var products = await _productService.SearchProductsAsync(searchTerm, categoryId, sortOrder);
            var categories = await _productService.GetCategoryOptionsAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);

            ViewData["CurrentFilter"] = searchTerm;
            ViewData["CurrentCategory"] = categoryId;
            ViewData["CurrentSort"] = sortOrder;
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productService.GetProductDetailsByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [Authorize(Roles = "Seller,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetProductsBySeller()
        {
            var userId = _userManager.GetUserId(User);
            var products = await _productService.GetBySellerIdAsync(userId);
            return View(products);
        }

        [Authorize(Roles = "Seller,Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = await _productService.GetCreateFormAsync();
            return View(viewModel);
        }

        [Authorize(Roles = "Seller,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var formModel = await _productService.GetCreateFormAsync();
                viewModel.AvailableCategories = formModel.AvailableCategories;
                return View(viewModel);
            }

            var SellerId = _userManager.GetUserId(User);

            await _productService.CreateAsync(viewModel, SellerId);
            TempData["SuccessMessage"] = "Product created successfully!";
            return RedirectToAction(nameof(GetProductsBySeller));
        }

        [Authorize(Roles = "Seller,Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var OwnershipResult = await CheckOwnershipAsync(id);
            if (OwnershipResult != null)
            {
                return OwnershipResult;
            }

            var viewModel = await _productService.GetEditFormAsync(id);
            return View(viewModel);
        }

        [Authorize(Roles = "Seller,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return BadRequest();
            }
            var OwnershipResult = await CheckOwnershipAsync(id);
            if (OwnershipResult != null)
            {
                return OwnershipResult;
            }
            if (!ModelState.IsValid)
            {
                await RepopulateCategoriesAsync(viewModel);
                return View(viewModel);
            }
            try
            {
                await _productService.UpdateAsync(viewModel, id);
                TempData["SuccessMessage"] = "Product updated successfully!";
                return RedirectToAction(nameof(GetProductsBySeller));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await RepopulateCategoriesAsync(viewModel);
                return View(viewModel);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the product.");
                await RepopulateCategoriesAsync(viewModel);
                return View(viewModel);
            }
        }

        [Authorize(Roles = "Seller,Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var OwnershipResult = await CheckOwnershipAsync(id);
            if (OwnershipResult != null)
            {
                return OwnershipResult;
            }
            var product = await _productService.GetProductDetailsByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [Authorize(Roles = "Seller,Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var OwnershipResult = await CheckOwnershipAsync(id);
            if (OwnershipResult != null)
            {
                return OwnershipResult;
            }
            await _productService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Product deleted successfully!";
            return RedirectToAction(nameof(GetProductsBySeller));
        }

        [Authorize(Roles = "Seller,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStock(int id, int newQuantity)
        {
            var OwnershipResult = await CheckOwnershipAsync(id);
            if (OwnershipResult != null)
            {
                return OwnershipResult;
            }
            try
            {
                await _productService.UpdateStockAsync(id, newQuantity);
                TempData["SuccessMessage"] = "Product stock updated successfully!";
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the product stock.";
            }
            return RedirectToAction(nameof(GetProductsBySeller));
        }


        private async Task<IActionResult?> CheckOwnershipAsync(int productId)
        {
            var ownerId = await _productService.GetOwnerIdAsync(productId);

            if (string.IsNullOrEmpty(ownerId))
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);

            if (!User.IsInRole("Admin") && ownerId != currentUserId)
            {
                return Forbid();
            }

            return null;
        }

        private async Task RepopulateCategoriesAsync(ProductEditViewModel viewModel)
        {
            var categories = await _productService.GetCreateFormAsync();
            viewModel.AvailableCategories = categories.AvailableCategories;
        }
    }
}
