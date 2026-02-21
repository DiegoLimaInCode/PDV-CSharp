using PDVCSharp.Data.Context;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Interfaces;

namespace PDVCSharp.Data.Repositories
{
    // UserRepository herda de Repository<Usuario> e implementa IUserRepository
    // Já possui todos os métodos CRUD (Add, Update, Delete, Where, etc.) herdados
    //
    // 💡 DICA: Se precisar de uma consulta específica de usuário (ex: buscar por login),
    //    adicione o método aqui. Ex:
    //    public async Task<Usuario?> GetByLogin(string login) => ...
    public class UserRepository : Repository<Usuario>, IUserRepository
    {
        // Construtor: recebe o AppDbContext e repassa para a classe base (Repository<Usuario>)
        // ": base(context)" = chama o construtor do pai passando o contexto
        public UserRepository(AppDbContext context) : base(context)
        {
        }
    }
}
