using PDVCSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PDVCSharp.Domain.Interfaces
{
    // IUserRepository herda de IRepository<Usuario>
    // Já possui todos os métodos do IRepository (GetAll, Add, Where, etc.) para Usuario.
    //
    // 💡 DICA: Mesmo vazia, essa interface é útil porque:
    //    1. Permite adicionar métodos exclusivos de usuário no futuro (ex: GetByLogin)
    //    2. Facilita a injeção de dependência — você injeta IUserRepository, não IRepository<Usuario>
    public interface IUserRepository : IRepository<Usuario>
    {
        Task<bool> DeleteByLoginHard(string login);
    }
}
