using PDVCSharp.Data.Context;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Interfaces;

namespace PDVCSharp.Data.Repositories
{
    // UserRepository herda de Repository<Usuario> — já possui todos os métodos CRUD herdados
    // 💡 DICA: Se precisar de uma consulta específica de usuário, adicione aqui.
    //    Ex: public async Task<Usuario?> GetByLogin(string login) => ...
    public class UserRepository : Repository<Usuario>, IUserRepository
    {
        // Construtor: recebe o AppDbContext e repassa para a classe base
        // ": base(context)" = chama o construtor do pai passando o contexto
        public UserRepository(AppDbContext context) : base(context)
        {
        }
    }
}
