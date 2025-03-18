using System.Text.Json.Serialization;

namespace fat_talkBack.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; }  // ✅ Added missing Location
        public string ProductType { get; set; } // ✅ Added missing ProductType
        public string Image { get; set; } // ✅ Added missing Image

        // Foreign key reference to User
        public int UserId { get; set; }
        
        [JsonIgnore] // Prevents circular reference
        public SignUpUser User { get; set; }
    }
}

