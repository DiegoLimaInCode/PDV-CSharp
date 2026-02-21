using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PDVCSharp.Domain.Interfaces
{
    // IRepository<T> é uma INTERFACE genérica — define um "contrato" que toda classe repositório deve seguir.
    // 💡 DICA: Interface = lista de métodos que a classe DEVE implementar, sem dizer COMO.
    //    Isso permite trocar a implementação (ex: de MySQL para PostgreSQL) sem mudar o resto do código.
    //
    // "where T : class" = T só pode ser uma classe (não pode ser int, bool, etc.)
    // <T> = Genérico — funciona com qualquer tipo (Produto, Usuario, etc.)
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Retorna todas as entidades do tipo T (excluindo as deletadas via soft delete).
        /// </summary>
        IQueryable<T> GetAll();

        /// <summary>
        /// Retorna uma entidade pelo seu ID (Guid).
        /// </summary>
        // 💡 DICA: Task<T?> = método assíncrono que pode retornar null (o "?" indica nullable)
        Task<T?> GetById(Guid id);

        /// <summary>
        /// Adiciona uma nova entidade no banco e retorna o ID gerado.
        /// </summary>
        Task<Guid> Add(T entity);

        /// <summary>
        /// Atualiza uma entidade existente no banco.
        /// </summary>
        Task Update(T entity);

        /// <summary>
        /// Deleta uma entidade (soft delete — não remove do banco).
        /// </summary>
        Task Delete(T entity);

        /// <summary>
        /// Salva todas as alterações pendentes no banco de dados.
        /// </summary>
        // 💡 DICA: O EF Core acumula mudanças em memória até você chamar Commit/SaveChanges.
        Task Commit();

        /// <summary>
        /// Filtra entidades usando uma expressão lambda (ex: x => x.Nome == "Banana").
        /// </summary>
        // 💡 DICA: IQueryable permite encadear filtros antes de ir ao banco.
        //    A consulta SQL só é gerada quando você chama .ToList(), .FirstOrDefault(), etc.
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
    }
}
