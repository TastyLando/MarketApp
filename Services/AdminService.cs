using MongoDB.Driver;
using Market.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

using MongoDB.Bson;

namespace Market.Services
{
    public class AdminService
    {
        private readonly IMongoCollection<Admin> _admins;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IConfiguration config, ILogger<AdminService> logger)
        {
            _logger = logger;
            var client = new MongoClient(config.GetSection("MongoDBSettings:ConnectionString").Value);
            var database = client.GetDatabase(config.GetSection("MongoDBSettings:DatabaseName").Value);
            _admins = database.GetCollection<Admin>("Admins");

            CreateIndexes().GetAwaiter().GetResult();
        }

        private async Task CreateIndexes()
        {
            await _admins.Indexes.CreateOneAsync(
                new CreateIndexModel<Admin>(
                    Builders<Admin>.IndexKeys.Ascending(x => x.Username),
                    new CreateIndexOptions { Unique = true }
                )
            );
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        public async Task<bool> ValidateAdmin(string username, string password)
        {
            try 
            {
                var admin = await GetAdminByUsername(username);
                if (admin != null && admin.Password == HashPassword(password))
                {
                    await UpdateLastLoginDate(admin.Id);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Giriş hatası: {ex.Message}");
                return false;
            }
        }

        private async Task UpdateLastLoginDate(string adminId)
        {
            var update = Builders<Admin>.Update.Set(a => a.LastLoginDate, DateTime.Now);
            await _admins.UpdateOneAsync(x => x.Id == adminId, update);
        }

        public async Task<Admin> GetAdminByUsername(string username)
        {
            return await _admins.Find(x => x.Username == username).FirstOrDefaultAsync();
        }

        public async Task CreateInitialAdmin()
        {
            try
            {
                // Önce koleksiyonu temizle (test için)
                await _admins.DeleteManyAsync(Builders<Admin>.Filter.Empty);

                var hashedPassword = HashPassword("123456");
                var admin = new Admin
                {
                    Username = "admin",
                    Password = hashedPassword,
                    LastLoginDate = null
                };

                await _admins.InsertOneAsync(admin);
                _logger.LogInformation("Admin kullanıcısı oluşturuldu/güncellendi.");
                
                // Oluşturulan admin bilgilerini kontrol et
                var createdAdmin = await GetAdminByUsername("admin");
                if (createdAdmin != null)
                {
                    _logger.LogInformation($"Admin kullanıcısı doğrulandı. Username: {createdAdmin.Username}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Admin oluşturma hatası: {ex.Message}");
                throw;
            }
        }

        public async Task ResetAdminPassword()
        {
            try
            {
                var admin = await _admins.Find(x => x.Username == "admin").FirstOrDefaultAsync();
                if (admin != null)
                {
                    var hashedPassword = HashPassword("123456");
                    var update = Builders<Admin>.Update.Set(x => x.Password, hashedPassword);
                    await _admins.UpdateOneAsync(x => x.Id == admin.Id, update);
                    Console.WriteLine("Admin şifresi başarıyla sıfırlandı.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Şifre sıfırlama hatası: {ex.Message}");
            }
        }

        public async Task<bool> IsMongoDbConnected()
        {
            try
            {
                await _admins.Database.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    
}
