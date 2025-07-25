using Microsoft.AspNetCore.Http;

namespace fat_talkBack.Models
{
    public class ItemUploadDto
    {
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; }
        public string ProductType { get; set; }
        public int UserId { get; set; }
        public IFormFile ImageFile { get; set; } // only used for receving uploaded files
    }
}