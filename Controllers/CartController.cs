using Microsoft.AspNetCore.Mvc;
using Market.Services;
using Market.Models;

namespace Market.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var cart = await _cartService.GetCart();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(string productId, int quantity = 1)
        {
            try
            {
                await _cartService.AddToCart(productId, quantity);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(string productId)
        {
            await _cartService.RemoveFromCart(productId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(string productId, int quantity)
        {
            await _cartService.UpdateQuantity(productId, quantity);
            var cart = await _cartService.GetCart();
            return Json(new { success = true, total = cart.TotalAmount });
        }
    }
} 