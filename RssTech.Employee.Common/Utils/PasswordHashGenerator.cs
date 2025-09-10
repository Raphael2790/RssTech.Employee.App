using System.Security.Cryptography;

namespace RssTech.Employee.Common.Utils;

public static class PasswordHashGenerator
{
    public static string HashPassword(string password)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}
