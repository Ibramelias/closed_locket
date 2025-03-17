using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class SignUpUser
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPass { get; set; }
    public string Phone { get; set; }
 public List<Item> Items { get; set; } = new List<Item>(); 

}