using Microsoft.EntityFrameworkCore;
using PDVCSharp.Application.Security;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Exceptions;
using PDVCSharp.Domain.Interfaces;

namespace PDVCSharp.Application.Services;

public class UsuarioService
{
    private readonly IUserRepository _userRepository;

    public UsuarioService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<string>> ListarLoginsAsync(string loginExcluido)
    {
        return await _userRepository
            .Where(u => u.Login.ToLower() != loginExcluido.ToLower())
            .OrderBy(u => u.Login)
            .Select(u => u.Login)
            .ToListAsync();
    }

    public async Task CadastrarAsync(string nome, string login, string senha, Cargo cargo)
    {
        if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(senha))
        {
            throw new DomainException("Nome, login e senha são obrigatórios.");
        }

        if (await _userRepository.ExistsByLogin(login))
        {
            throw new DomainException("Já existe um usuário com esse login.");
        }

        await _userRepository.Add(new Usuario
        {
            Name = nome.Trim(),
            Login = login.Trim(),
            Password = PasswordHasher.Hash(senha),
            Cargo = cargo
        });
    }

    public async Task ExcluirPorLoginAsync(string login)
    {
        var removido = await _userRepository.DeleteByLoginHard(login);
        if (!removido)
        {
            throw new DomainException("Usuário não encontrado para exclusão.");
        }
    }
}
