using System.ComponentModel.DataAnnotations;

namespace Model;

public class LoginModel
{
    [Required]
    public string login {get; set;}
    [Required]
    public string password {get; set;}
}