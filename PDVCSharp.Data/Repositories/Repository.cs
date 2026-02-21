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
    // Repository<T> \u00e9 a implementa\u00e7\u00e3o CONCRETA da interface IRepository<T>
    // Cont\u00e9m a l\u00f3gica real de acesso ao banco (CRUD) usando Entity Framework.
    // "where T : BaseEntity" = T s\u00f3 pode ser uma entidade que herda de BaseEntity
    // \ud83d\udca1 DICA: Esse padr\u00e3o se chama "Repository Pattern" \u2014 centraliza todo o acesso
    //    ao banco em um s\u00f3 lugar, facilitando manuten\u00e7\u00e3o e testes.
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        // _context = refer\u00eancia ao banco de dados (AppDbContext)
        // "protected" = acess\u00edvel nesta classe e nas filhas (ex: UserRepository)
        // "readonly" = s\u00f3 pode ser atribu\u00eddo no construtor (seguran\u00e7a)
        protected readonly AppDbContext _context;

        // _dbSet = refer\u00eancia direta \u00e0 tabela do tipo T no banco
        // Ex: se T = Usuario, _dbSet \u00e9 a tabela Usuarios
        protected readonly DbSet<T> _dbSet;

        // Construtor: recebe o AppDbContext via inje\u00e7\u00e3o de depend\u00eancia
        // \ud83d\udca1 DICA: Inje\u00e7\u00e3o de depend\u00eancia = o objeto \u00e9 \"injetado\" de fora pelo cont\u00eainer de DI.
        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>(); // Obt\u00e9m o DbSet correspondente ao tipo T
        }

        // Adiciona uma nova entidade no banco e salva automaticamente
        // "virtual" = permite que classes filhas sobrescrevam esse m\u00e9todo
        // "async Task" = m\u00e9todo ass\u00edncrono (n\u00e3o trava a thread enquanto espera o banco)
        public virtual async Task<Guid> Add(T entity)
        {
            try
            {
                var obj = await _dbSet.AddAsync(entity); // Adiciona ao DbSet (memória)
                await Commit();                           // Salva no banco (INSERT SQL)

                return obj.Entity.Id; // Retorna o Id gerado
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar entidade {typeof(T).Name} com ID {entity.Id}: {ex.Message}", ex);
            }
        }

        // Atualiza uma entidade existente e salva automaticamente
        public virtual async Task Update(T entity)
        {
            _dbSet.Update(entity); // Marca como "modificada" no EF Core
            await Commit();        // Salva (UPDATE SQL)
        }

        // Atualiza múltiplas entidades de uma vez (sem commit automático)
        // 💡 DICA: Útil para operações em lote — chame Commit() manualmente depois
        public virtual Task UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
            return Task.CompletedTask;
        }

        // Aplica soft delete — não remove do banco, apenas marca IsDeleted = true
        public virtual async Task Delete(T entity)
        {
            entity.DeleteMethod(); // Chama o método de BaseEntity que marca IsDeleted = true
            _context.Entry(entity).State = EntityState.Modified; // Informa ao EF que mudou
            await Commit();
        }

        // Salva todas as alterações pendentes no banco com retry para conflitos
        // 💡 DICA: Concorrência = quando dois usuários alteram o mesmo registro ao mesmo tempo.
        public virtual async Task Commit()
        {
            const int maxRetries = 3; // Número máximo de tentativas
            var delay = TimeSpan.FromMilliseconds(100); // Espera entre tentativas

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    // Log das entidades que serão salvas para diagnóstico
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

                    var changes = await _context.SaveChangesAsync();
                    if (changes > 0)
                    {
                        Console.WriteLine($"💾 Commit successful: {changes} changes saved to database");
                    }
                    return;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (attempt == maxRetries - 1)
                    {
                        Console.WriteLine($"❌ Commit failed after {maxRetries} attempts: {ex.Message}");
                        throw new Exception($"Erro ao salvar mudanças no banco após {maxRetries} tentativas (conflito de concorrência): {ex.Message}", ex);
                    }

                    // Backoff exponencial: 100ms, 200ms, 400ms
                    await Task.Delay(delay);
                    delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2);

                    // ⚠️ PROBLEMA: ReloadAsync sobrescreve os valores que definimos!
                    // Em vez de recarregar, vamos usar "client wins" - manter nossos valores
                    foreach (var entry in ex.Entries)
                    {
                        // Opção 1: Client wins - usar nossos valores (em vez de ReloadAsync)
                        var proposedValues = entry.CurrentValues;
                        var databaseValues = await entry.GetDatabaseValuesAsync();

                        if (databaseValues != null)
                        {
                            // Manter os valores propostos (nossos valores)
                            entry.OriginalValues.SetValues(databaseValues);
                            Console.WriteLine($"⚠️ Conflito resolvido com 'client wins' para {entry.Entity.GetType().Name}");
                        }
                    }

                    Console.WriteLine($"⚠️ Conflito de concorrência detectado. Tentativa {attempt + 1}/{maxRetries}. Retry em {delay.TotalMilliseconds}ms");
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("A second operation was started"))
                {
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
                .Where(e => !e.IsDeleted);
        }

        // Busca uma entidade específica pelo ID (somente ativas)
        public virtual async Task<T?> GetById(Guid id)
        {
            return await _dbSet.AsNoTracking()
                .Where(e => !e.IsDeleted)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        // Filtra entidades usando uma expressão lambda
        // Ex: Where(u => u.Login == "admin") gera: SELECT * FROM Usuarios WHERE Login = 'admin'
        // 💡 DICA: Retorna IQueryable — a consulta SQL só executa ao consumir (.ToList(), etc.)
        public virtual IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).Where(e => !e.IsDeleted);
        }
    }
}
