using Microsoft.EntityFrameworkCore;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Interfaces;
using System.Text.Json;

namespace PDVCSharp.Application.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Login(string usuario, string senha)
        {
            var user = await _userRepository.Where(_user => _user.Login.ToLower() == usuario.ToLower()).FirstOrDefaultAsync();

            if (user is null)
            {
                throw new Exception("Usuário não localizado");
            }

            if (user.Password != senha)
            {
                throw new Exception("Senha incorreta");
            }

            return true;
        }
    }
}
