using Exadel.ReportHub.Common;

namespace Exadel.ReportHub.Tests.Common;

[TestFixture]
public class PasswordHasherTests
{
    private const string Password = "TestPassword123!";
    private const string PasswordHash = "U97MW3kDru47iKqVXDCy8+bamGNa8jCoI28amjFozwzQMItKxzilV43JhvYwRoxMJtKNc/8jHD3iuDuj7xFVnA==";
    private const string PasswordSalt = "aGVsbG9Xb3JsZA==";

    [Test]
    public void CreatePasswordHash_PasswordProvided_UniqueHashAndSaltGenerated()
    {
        var (hashedPassword, salt) = PasswordHasher.CreatePasswordHash(Password);
        Assert.That(hashedPassword, Is.Not.Null);
        Assert.That(salt, Is.Not.Null);
        Assert.That(hashedPassword.Length, Is.GreaterThan(80));
        Assert.That(salt.Length, Is.GreaterThan(16));
        Assert.DoesNotThrow(() => Convert.FromBase64String(hashedPassword));
        Assert.DoesNotThrow(() => Convert.FromBase64String(salt));
    }

    [Test]
    public void GetPasswordHash_ValidPasswordAndSalt_HashedValueReturned()
    {
        var hashedValue = PasswordHasher.GetPasswordHash(Password, PasswordSalt);
        Assert.That(hashedValue, Is.Not.Null);
        Assert.That(hashedValue, Is.EqualTo(PasswordHash));
    }
}
