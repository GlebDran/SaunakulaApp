using System.Security.Cryptography;
using System.Text;

namespace SaunakulaApp.Services;

public static class PasswordService
{
    public static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}
