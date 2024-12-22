using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Market.Services;
using Market.Models;
using Market.Filters;
using Microsoft.Extensions.Logging;

namespace Market.Controllers
{
    public class AdminController : Controller
    {
        private readonly ProductService _productService;
        private readonly AdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ProductService productService, AdminService adminService, ILogger<AdminController> logger)
        {
            _productService = productService;
            _adminService = adminService;
            _logger = logger;
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("AdminUsername") != null)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            _logger.LogInformation($"Giriş denemesi - Kullanıcı: {username}");
            
            var admin = await _adminService.GetAdminByUsername(username);
            if (admin == null)
            {
                _logger.LogWarning($"Kullanıcı bulunamadı: {username}");
                ModelState.AddModelError("", "Geçersiz kullanıcı adı veya şifre");
                return View();
            }

            _logger.LogInformation($"Kullanıcı bulundu, şifre kontrolü yapılıyor");

            if (await _adminService.ValidateAdmin(username, password))
            {
                _logger.LogInformation($"Giriş başarılı: {username}");
                HttpContext.Session.SetString("AdminUsername", username);
                return RedirectToAction("Dashboard");
            }
            
            _logger.LogWarning($"Şifre yanlış: {username}");
            ModelState.AddModelError("", "Geçersiz kullanıcı adı veya şifre");
            return View();
        }

        [AdminAuth]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var products = await _productService.GetAllProducts();
                ViewBag.TotalProducts = products.Count;
                ViewBag.TotalStock = products.Sum(p => p.Stock);
                ViewBag.TotalValue = products.Sum(p => p.Price * p.Stock);
                
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Dashboard verilerini getirme hatası: {ex.Message}");
                ViewBag.TotalProducts = 0;
                ViewBag.TotalStock = 0;
                ViewBag.TotalValue = 0;
                return View();
            }
        }

        [AdminAuth]
        public async Task<IActionResult> Products()
        {
            try
            {
                var products = await _productService.GetAllProducts();
                _logger.LogInformation($"Toplam {products.Count} ürün listelendi.");
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ürünleri getirme hatası: {ex.Message}");
                TempData["Error"] = "Ürünler yüklenirken bir hata oluştu.";
                return View(new List<Product>());
            }
        }

        [AdminAuth]
        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        [AdminAuth]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    product.Id = null;
                    product.CreatedAt = DateTime.Now;
                    
                    var success = await _productService.CreateProduct(product);
                    
                    if (success)
                    {
                        _logger.LogInformation($"Ürün başarıyla eklendi: {product.Name}");
                        TempData["Success"] = "Ürün başarıyla eklendi.";
                        return RedirectToAction(nameof(Products));
                    }
                }
                
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _logger.LogError($"Model hatası: {error.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ürün ekleme hatası: {ex.Message}");
                ModelState.AddModelError("", "Ürün eklenirken bir hata oluştu.");
            }

            return View(product);
        }

        [AdminAuth]
        public async Task<IActionResult> EditProduct(string id)
        {
            try
            {
                var product = await _productService.GetProductById(id);
                if (product == null)
                {
                    TempData["Error"] = "Ürün bulunamadı.";
                    return RedirectToAction(nameof(Products));
                }
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ürün düzenleme sayfası hatası: {ex.Message}");
                TempData["Error"] = "Ürün yüklenirken bir hata oluştu.";
                return RedirectToAction(nameof(Products));
            }
        }

        [HttpPost]
        [AdminAuth]
        public async Task<IActionResult> EditProduct(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var success = await _productService.UpdateProduct(product);
                    if (success)
                    {
                        TempData["Success"] = "Ürün başarıyla güncellendi.";
                        return RedirectToAction(nameof(Products));
                    }
                }
                
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _logger.LogError($"Model hatası: {error.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ürün güncelleme hatası: {ex.Message}");
                ModelState.AddModelError("", "Ürün güncellenirken bir hata oluştu.");
            }

            return View(product);
        }

        [HttpPost]
        [AdminAuth]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            try
            {
                var success = await _productService.DeleteProduct(id);
                if (success)
                {
                    TempData["Success"] = "Ürün başarıyla silindi.";
                }
                else
                {
                    TempData["Error"] = "Ürün silinirken bir hata oluştu.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ürün silme hatası: {ex.Message}");
                TempData["Error"] = "Ürün silinirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Products));
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
