using System.Linq.Expressions;
using PDVCSharp.Application.Services;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Exceptions;
using PDVCSharp.Domain.Interfaces;

namespace PDVCSharp.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task Login_ComCredencialValida_RetornaUsuario()
    {
        var repo = new FakeUserRepository(new Usuario
        {
            Login = "caixa",
            Password = "caixa",
            Cargo = Cargo.Caixa,
            Name = "Caixa"
        });
        var service = new AuthService(repo);

        var user = await service.Login("caixa", "caixa");

        Assert.Equal("caixa", user.Login);
        Assert.StartsWith("pbkdf2$", user.Password);
    }

    [Fact]
    public async Task Login_ComSenhaErrada_LancaAutenticacao()
    {
        var repo = new FakeUserRepository(new Usuario { Login = "admin", Password = "admin", Name = "Admin" });
        var service = new AuthService(repo);

        await Assert.ThrowsAsync<AutenticacaoException>(() => service.Login("admin", "errada"));
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly Usuario _usuario;

        public FakeUserRepository(Usuario usuario) => _usuario = usuario;

        public Task<Usuario?> GetByLogin(string login)
            => Task.FromResult(login.Equals(_usuario.Login, StringComparison.OrdinalIgnoreCase) ? _usuario : null);

        public Task<bool> ExistsByLogin(string login) => Task.FromResult(false);
        public Task<bool> DeleteByLoginHard(string login) => Task.FromResult(false);
        public IQueryable<Usuario> GetAll() => throw new NotImplementedException();
        public Task<Usuario?> GetById(Guid id) => Task.FromResult<Usuario?>(_usuario);
        public Task<Guid> Add(Usuario entity) => Task.FromResult(entity.Id);
        public Task Update(Usuario entity) => Task.CompletedTask;
        public Task Delete(Usuario entity) => Task.CompletedTask;
        public Task Commit() => Task.CompletedTask;
        public IQueryable<Usuario> Where(Expression<Func<Usuario, bool>> predicate) => throw new NotImplementedException();
    }
}
