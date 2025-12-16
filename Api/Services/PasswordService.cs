using Api.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

public class PasswordService : IPasswordService
{
    public string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        byte[] key = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 32
        );

        string saltB64 = ToBase64Url(salt);
        string hashB64 = ToBase64Url(key);

        return $"{saltB64}.{hashB64}";
    }

    public bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2) return false;

        byte[] salt = FromBase64Url(parts[0]);
        byte[] storedKey = FromBase64Url(parts[1]);

        byte[] attemptKey = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 32
        );

        return CryptographicOperations.FixedTimeEquals(storedKey, attemptKey);
    }

    public string? ValidatePasswordComplexity(string password)
    {
        if (password.Length < 8) return "Password must be at least 8 characters.";
        if (!password.Any(char.IsDigit)) return "Password must contain a number.";
        if (!password.Any(char.IsUpper)) return "Password must contain an uppercase letter.";
        return null;
    }

    private static string ToBase64Url(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }

    private static byte[] FromBase64Url(string base64Url)
    {
        string padded = base64Url
            .Replace("-", "+")
            .Replace("_", "/");

        switch (padded.Length % 4)
        {
            case 2: padded += "=="; break;
            case 3: padded += "="; break;
        }

        return Convert.FromBase64String(padded);
    }
}