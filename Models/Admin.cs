using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Market.Models
{
    public class Admin
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        [BsonElement("Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur")]
        [BsonElement("Password")]
        public string Password { get; set; }

        [BsonElement("LastLoginDate")]
        public DateTime? LastLoginDate { get; set; }
    }
}
