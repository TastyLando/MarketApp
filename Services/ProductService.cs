using MongoDB.Driver;
using Market.Models;
using Microsoft.Extensions.Logging;

namespace Market.Services
{
    public class ProductService
    {
        private readonly IMongoCollection<Product> _products;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IConfiguration config, ILogger<ProductService> logger)
        {
            _logger = logger;
            var client = new MongoClient(config.GetSection("MongoDBSettings:ConnectionString").Value);
            var database = client.GetDatabase(config.GetSection("MongoDBSettings:DatabaseName").Value);
            _products = database.GetCollection<Product>("Products");
        }

        public async Task<List<Product>> GetAllProducts()
        {
            try
            {
                return await _products.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ürünleri getirme hatası: {ex.Message}");
                return new List<Product>();
            }
        }

        public async Task<Product> GetProductById(string id)
        {
            try
            {
                return await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ürün getirme hatası: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateProduct(Product product)
        {
            try
            {
                await _products.InsertOneAsync(product);
                _logger.LogInformation($"Yeni ürün eklendi: {product.Name}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ürün ekleme hatası: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            try
            {
                var result = await _products.ReplaceOneAsync(p => p.Id == product.Id, product);
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ürün güncelleme hatası: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteProduct(string id)
        {
            try
            {
                var result = await _products.DeleteOneAsync(p => p.Id == id);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ürün silme hatası: {ex.Message}");
                return false;
            }
        }
    }
}