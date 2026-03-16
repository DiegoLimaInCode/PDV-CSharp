using Microsoft.EntityFrameworkCore;
using PDVCSharp.Data.Context;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Interfaces;

namespace PDVCSharp.Data.Repositories
{
    public class UserRepository : Repository<Usuario>, IUserRepository
    {
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
