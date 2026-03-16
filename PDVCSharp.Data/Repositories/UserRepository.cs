using Microsoft.EntityFrameworkCore;
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

        public async Task<bool> DeleteByLoginHard(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return false;
            }

            var loginNormalizado = login.Trim().ToLower();
            var usuario = await _dbSet.FirstOrDefaultAsync(u => u.Login.ToLower() == loginNormalizado);
            if (usuario is null)
            {
                return false;
            }

            _dbSet.Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
