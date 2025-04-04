using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Exadel.ReportHub.Common;

public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 64;
    private const int Iterations = 100000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

    public static (string PasswordHash, string PasswordSalt) CreatePasswordHash(string password)
    {
        byte[] saltData = RandomNumberGenerator.GetBytes(SaltSize);
        string passwordSalt = Convert.ToBase64String(saltData);
        byte[] hashedData = Rfc2898DeriveBytes.Pbkdf2(password, saltData, Iterations, HashAlgorithm, HashSize);
        string passwordHash = Convert.ToBase64String(hashedData);
        return (passwordHash, passwordSalt);
    }

    public static string GetPasswordHash(string password, string passwordSalt)
    {
        byte[] saltData = Convert.FromBase64String(passwordSalt);
        byte[] hashedData = Rfc2898DeriveBytes.Pbkdf2(password, saltData, Iterations, HashAlgorithm, HashSize);

        return Convert.ToBase64String(hashedData);
    }
}
