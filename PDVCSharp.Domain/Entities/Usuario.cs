using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PDVCSharp.Domain.Entities
{
    // Usuario herda de BaseEntity — possui Id, CreatedAt, UpdatedAt, IsDeleted automaticamente
    // Representa um operador/funcionário que pode fazer login no PDV
    public class Usuario : BaseEntity
    {
        public string Login { get; set; }     // Nome de usuário para login (ex: "admin")
        public string Password { get; set; }  // Senha do usuário
        // 💡 DICA: Em produção, NUNCA armazene senhas em texto puro!
        //    Use hashing (ex: BCrypt) para proteger as senhas dos usuários.
    }

}
