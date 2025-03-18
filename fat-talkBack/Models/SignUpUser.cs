namespace fat_talkBack.Models
{
    public class SignUpUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPass { get; set; }
        public string Phone { get; set; }
        
        // Navigation property
        public List<Item> Items { get; set; } = new List<Item>();
    }
}
