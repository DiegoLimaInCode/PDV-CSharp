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
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<Guid> Add(T entity)
        {
            try
            {
                var obj = await _dbSet.AddAsync(entity);
                await Commit();
                return obj.Entity.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar entidade {typeof(T).Name} com ID {entity.Id}: {ex.Message}", ex);
            }
        }

        public virtual async Task Update(T entity)
        {
            _dbSet.Update(entity);
            await Commit();
        }

        public virtual Task UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
            return Task.CompletedTask;
        }

        public virtual async Task Delete(T entity)
        {
            entity.DeleteMethod();
            _context.Entry(entity).State = EntityState.Modified;
            await Commit();
        }

        public virtual async Task Commit()
        {
            const int maxRetries = 3;
            var delay = TimeSpan.FromMilliseconds(100);

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
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

                    await Task.Delay(delay);
                    delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2);

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

        public virtual IQueryable<T> GetAll()
        {
            return _dbSet.AsNoTracking()
                .Where(e => !e.IsDeleted);
        }

        public virtual async Task<T?> GetById(Guid id)
        {
            return await _dbSet.AsNoTracking()
                .Where(e => !e.IsDeleted)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public virtual IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).Where(e => !e.IsDeleted);
        }
    }
}
