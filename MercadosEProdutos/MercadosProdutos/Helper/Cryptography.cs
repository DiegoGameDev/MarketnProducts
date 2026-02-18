using System.Security.Cryptography;
using System.Text;

namespace Helper;

public static class Cryptography
{
    public static string SetHash(this string value)
    {
        var sha1 = SHA1.Create();
        var enconding = new ASCIIEncoding();
        var array = enconding.GetBytes(value);

        array = sha1.ComputeHash(array);

        var strHexa = new StringBuilder();

        foreach(var i in array)
        {
            strHexa.Append(i.ToString("x2"));
        }

        value = strHexa.ToString();
        return value;
    }
}