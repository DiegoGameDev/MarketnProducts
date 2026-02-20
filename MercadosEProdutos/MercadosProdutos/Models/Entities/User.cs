using System.ComponentModel.DataAnnotations;
using Enums;
using Helper;
using Microsoft.AspNetCore.Identity;

namespace DBModel;

public class User : IdentityUser
{
    public UserType userType {get; set;}
    public ICollection<MarketAssociated> marketAssociateds {get; set;}

    public bool IsValid(string _password)
    {
        _password = _password.SetHash();

        

        if (PasswordHash == _password) return true;

        return false;
    }
}

