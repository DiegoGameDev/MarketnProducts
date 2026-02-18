using System.ComponentModel.DataAnnotations;
using DBModel;

namespace Model;

public class RegisterModel
{
    [Required]
    public string name {get; set;}
    [Required]
    public string email {get; set;}
    public string? phone {get; set;}
    [Required]
    public string password {get; set;}

    public User ToUser()
    {
        return new User()
        {
          UserName = this.name,
          Email = this.email,
          NormalizedEmail = email.ToUpper(),
          PhoneNumber = this.phone,
          userType = Enums.UserType.Default,   
        };
    }
}