using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Item
{
    [Key]
    public int Id { get; set; }
    
    public string Description { get; set; }
    public int Price { get; set; }
    public string Location { get; set; }
    public string ProductType { get; set; }
    public string Image { get; set; }

    // Foreign Key to associate this item with a user
    public int UserId { get; set; }
    public SignUpUser User { get; set; }
}
