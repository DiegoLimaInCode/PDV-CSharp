using Microsoft.EntityFrameworkCore;
using PDVCSharp.Data.Context;
using PDVCSharp.Domain.Entities;
using PDVCSharp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PDVCSharp.Data.Repositories
{
    // Repository<T> é a implementação CONCRETA da interface IRepository<T>
    // Ele contém a lógica real de acesso ao banco (CRUD) usando Entity Framework.
    //
    // "where T : BaseEntity" = T só pode ser uma entidade que herda de BaseEntity
    // 💡 DICA: Esse padrão se chama "Repository Pattern" — centraliza todo o acesso
    //    ao banco em um só lugar, facilitando manutenção e testes.
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        // _context = referência ao banco de dados (AppDbContext)
        // "protected" = acessível nesta classe e nas classes filhas (ex: UserRepository)
        // "readonly" = só pode ser atribuído no construtor (segurança)
        protected readonly AppDbContext _context;

        // _dbSet = referência direta à tabela do tipo T no banco
        // Ex: se T = Usuario, _dbSet é a tabela Usuarios
        protected readonly DbSet<T> _dbSet;

        // Construtor: recebe o AppDbContext via injeção de dependência
        // 💡 DICA: Injeção de dependência = em vez de criar o objeto aqui dentro,
        //    ele é "injetado" de fora (pelo contêiner de DI). Isso facilita testes.
        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>(); // Obtém o DbSet correspondente ao tipo T
        }

        // Adiciona uma nova entidade no banco e salva automaticamente
        // "virtual" = permite que classes filhas sobrescrevam esse método
        // "async Task" = método assíncrono (não trava a thread enquanto espera o banco)
        public virtual async Task<Guid> Add(T entity)
        {
            try
            {
                // Adiciona a entidade ao DbSet (ainda em memória)
                var obj = await _dbSet.AddAsync(entity);
                // Salva no banco de dados (executa o INSERT SQL)
                await Commit();

                // Retorna o Id da entidade que foi salva
                return obj.Entity.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar entidade {typeof(T).Name} com ID {entity.Id}: {ex.Message}", ex);
            }
        }

        // Atualiza uma entidade existente no banco
        public virtual async Task Update(T entity)
        {
            // Marca a entidade como "modificada" no EF Core
            _dbSet.Update(entity);
            // Salva as mudanças (executa o UPDATE SQL)
            await Commit();
        }

        // Atualiza múltiplas entidades de uma vez (sem commit automático)
        // 💡 DICA: Útil para operações em lote — chame Commit() manualmente depois
        public virtual Task UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
            return Task.CompletedTask; // Retorna uma Task já completada (não é async)
        }

        // Aplica soft delete — não remove do banco, apenas marca IsDeleted = true
        public virtual async Task Delete(T entity)
        {
            entity.DeleteMethod(); // Chama o método de BaseEntity que marca IsDeleted = true
            _context.Entry(entity).State = EntityState.Modified; // Informa ao EF que a entidade mudou
            await Commit();
        }

        // Salva todas as alterações pendentes no banco de dados
        // Inclui lógica de retry (tentativas) para conflitos de concorrência
        //
        // 💡 DICA: Concorrência = quando dois usuários tentam alterar o mesmo registro ao mesmo tempo.
        //    O retry tenta novamente caso ocorra um conflito.
        public virtual async Task Commit()
        {
            const int maxRetries = 3; // Número máximo de tentativas
            var delay = TimeSpan.FromMilliseconds(100); // Tempo de espera entre tentativas

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    // Log: mostra no console quais entidades serão salvas (para debug)
                    var modifiedEntries = _context.ChangeTracker.Entries()
                        .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added)
                        .ToList();

                    if (modifiedEntries.Any())
                    {
                        foreach (var entry in modifiedEntries)
                        {
                            var entityType = entry.Entity.GetType().Name;
                            var modifiedProps = entry.Properties
                                .Where(p => p.IsModified)
                                .Select(p => $"{p.Metadata.Name}='{p.CurrentValue}'")
                                .ToList();
                            Console.WriteLine($"💾 [COMMIT] {entityType} ({entry.State}): {string.Join(", ", modifiedProps.Take(5))}...");
                        }
                    }

                    // SaveChangesAsync = envia todas as mudanças para o banco (INSERT/UPDATE/DELETE)
                    var changes = await _context.SaveChangesAsync();
                    if (changes > 0)
                    {
                        Console.WriteLine($"💾 Commit successful: {changes} changes saved to database");
                    }
                    return; // Sucesso — sai do loop
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Conflito de concorrência: outro processo alterou o mesmo registro
                    if (attempt == maxRetries - 1)
                    {
                        Console.WriteLine($"❌ Commit failed after {maxRetries} attempts: {ex.Message}");
                        throw new Exception($"Erro ao salvar mudanças no banco após {maxRetries} tentativas (conflito de concorrência): {ex.Message}", ex);
                    }

                    // Backoff exponencial: espera cada vez mais entre tentativas (100ms, 200ms, 400ms)
                    // 💡 DICA: Esse padrão evita sobrecarregar o banco com tentativas simultâneas.
                    await Task.Delay(delay);
                    delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2);

                    // Estratégia "client wins": mantém os valores do cliente (nossos valores)
                    // e atualiza os valores originais com os do banco para evitar novo conflito
                    foreach (var entry in ex.Entries)
                    {
                        var proposedValues = entry.CurrentValues;
                        var databaseValues = await entry.GetDatabaseValuesAsync();

                        if (databaseValues != null)
                        {
                            entry.OriginalValues.SetValues(databaseValues);
                            Console.WriteLine($"⚠️ Conflito resolvido com 'client wins' para {entry.Entity.GetType().Name}");
                        }
                    }

                    Console.WriteLine($"⚠️ Conflito de concorrência detectado. Tentativa {attempt + 1}/{maxRetries}. Retry em {delay.TotalMilliseconds}ms");
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("A second operation was started"))
                {
                    // Erro: duas operações usando o mesmo DbContext ao mesmo tempo
                    // 💡 DICA: DbContext NÃO é thread-safe. Nunca use o mesmo contexto em paralelo.
                    if (attempt == maxRetries - 1)
                    {
                        Console.WriteLine($"❌ Commit failed after {maxRetries} attempts: {ex.Message}");
                        throw new Exception($"Erro ao salvar mudanças no banco: Operação concorrente detectada no DbContext. Certifique-se de que apenas uma operação usa o contexto por vez.", ex);
                    }

                    await Task.Delay(delay);
                    delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2);
                    Console.WriteLine($"⚠️ Operação concorrente detectada. Tentativa {attempt + 1}/{maxRetries}. Retry em {delay.TotalMilliseconds}ms");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Commit failed: {ex.Message}");
                    throw new Exception($"Erro ao salvar mudanças no banco: {ex.Message}", ex);
                }
            }
        }

        // Retorna todas as entidades ativas (não deletadas)
        // AsNoTracking() = não rastreia mudanças (melhor performance para leitura)
        // 💡 DICA: Use AsNoTracking quando você só quer LER dados, sem modificar.
        public virtual IQueryable<T> GetAll()
        {
            return _dbSet.AsNoTracking()
                .Where(e => !e.IsDeleted); // Filtra os soft-deleted
        }

        // Busca uma entidade específica pelo ID (somente ativas)
        public virtual async Task<T?> GetById(Guid id)
        {
            return await _dbSet.AsNoTracking()
                .Where(e => !e.IsDeleted)
                .FirstOrDefaultAsync(e => e.Id == id); // Retorna o primeiro que bater ou null
        }

        // Filtra entidades usando uma expressão lambda
        // Ex: Where(u => u.Login == "admin") gera: SELECT * FROM Usuarios WHERE Login = 'admin'
        // 💡 DICA: O retorno é IQueryable — a consulta SQL só executa quando você consome o resultado
        //    (com .ToList(), .FirstOrDefault(), etc.)
        public virtual IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).Where(e => !e.IsDeleted);
        }
    }
}
