using PDVCSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PDVCSharp.Domain.Interfaces
{
    public interface IUserRepository : IRepository<Usuario>
    {
        Task<bool> DeleteByLoginHard(string login);
    }
}
