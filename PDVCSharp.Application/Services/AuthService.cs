using PDVCSharp.Application.Security;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Exceptions;
using PDVCSharp.Domain.Interfaces;

namespace PDVCSharp.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Usuario> Login(string usuario, string senha)
    {
        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(senha))
        {
            throw new AutenticacaoException("Informe usuário e senha.");
        }

        var user = await _userRepository.GetByLogin(usuario)
            ?? throw new AutenticacaoException("Usuário não localizado");

        if (!PasswordHasher.Verify(senha, user.Password))
        {
            throw new AutenticacaoException("Senha incorreta");
        }

        if (!PasswordHasher.IsHashed(user.Password))
        {
            user.Password = PasswordHasher.Hash(senha);
            await _userRepository.Update(user);
        }

        return user;
    }
}
