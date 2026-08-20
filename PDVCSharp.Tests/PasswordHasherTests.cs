using PDVCSharp.Application.Security;

namespace PDVCSharp.Tests;

public class PasswordHasherTests
{
    [Fact]
    public void Hash_NaoGuardaTextoPuro()
    {
        var hash = PasswordHasher.Hash("admin");
        Assert.NotEqual("admin", hash);
        Assert.True(PasswordHasher.IsHashed(hash));
    }

    [Fact]
    public void Verify_AceitaSenhaCorreta()
    {
        var hash = PasswordHasher.Hash("caixa");
        Assert.True(PasswordHasher.Verify("caixa", hash));
        Assert.False(PasswordHasher.Verify("outra", hash));
    }

    [Fact]
    public void Verify_AceitaSenhaLegadaEmTextoPuro()
    {
        Assert.True(PasswordHasher.Verify("admin", "admin"));
    }
}
