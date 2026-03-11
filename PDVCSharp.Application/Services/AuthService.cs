using Microsoft.EntityFrameworkCore;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Interfaces;
using System.Text.Json;

namespace PDVCSharp.Application.Services
{
    // AuthService = serviço responsável pela autenticação (login) do usuário
    // Fica na camada Application porque contém REGRAS DE NEGÓCIO (não acessa o banco diretamente)
    //
    // 💡 DICA: A arquitetura em camadas funciona assim:
    //    WPF (tela) → Application (regras) → Data (banco) → Domain (modelos)
    //    Cada camada só conhece a camada abaixo dela.
    public class AuthService
    {
        // Repositório de usuários — injetado via construtor (injeção de dependência)
        // "readonly" = não pode ser alterado após a atribuição no construtor
        private readonly IUserRepository _userRepository;

        // Construtor: o contêiner de DI injeta automaticamente o IUserRepository
        // 💡 DICA: O AuthService não sabe se o repositório usa MySQL, PostgreSQL ou arquivo JSON.
        //    Ele só conhece a INTERFACE (IUserRepository). Isso é "inversão de dependência".
        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Método de login: verifica se o usuário existe e se a senha está correta
        // "async Task<bool>" = método assíncrono que retorna true/false
        // 💡 DICA: async/await permite que o app continue responsivo enquanto espera o banco.
        public async Task<bool> Login(string usuario, string senha)
        {
            // Busca o usuário no banco pelo login (case-insensitive com ToLower)
            // Where() retorna IQueryable, FirstOrDefaultAsync() executa a consulta no banco
            // 💡 DICA: "await" pausa este método até o banco responder, sem travar a thread.
            var user = await _userRepository.Where(_user => _user.Login.ToLower() == usuario.ToLower()).FirstOrDefaultAsync();

            // Se não encontrou nenhum usuário com esse login
            if (user is null)
            {
                throw new Exception("Usuário não localizado");
            }

            // Se a senha não confere
            if (user.Password != senha)
            {
                throw new Exception("Senha incorreta");
            }

            return true; // Login bem-sucedido!
        }


    }
}
