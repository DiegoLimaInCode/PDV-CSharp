using System.Security.Cryptography;
using System.Text;

namespace PDVCSharp.Application.Security;

public static class PasswordHasher
{
    public static bool IsHashed(string value)
        => !string.IsNullOrWhiteSpace(value) && value.StartsWith("pbkdf2$", StringComparison.Ordinal);

    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return $"pbkdf2${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string password, string stored)
    {
        if (!IsHashed(stored))
        {
            return stored == password;
        }

        var parts = stored.Split('$');
        if (parts.Length != 3)
        {
            return false;
        }

        var salt = Convert.FromBase64String(parts[1]);
        var expected = Convert.FromBase64String(parts[2]);
        var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
