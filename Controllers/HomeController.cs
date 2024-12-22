using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Market.Models;
using Market.Services;

namespace Market.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
   private readonly ProductService _productService;
    public HomeController(ILogger<HomeController> logger, ProductService productService)
   {
       _logger = logger;
       _productService = productService;
   }
    public async Task<IActionResult> Index()
   {
       var products = await _productService.GetAllProducts();
       return View(products);
   }
    
     public IActionResult Contact()
   {
       return View();
   }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
