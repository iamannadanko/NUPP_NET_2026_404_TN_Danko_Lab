using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Infrastructure.Repositories
{
    // Інтерфейс патерну Репозиторій відповідно до вимог завдання 4
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByIdAsync(Guid id); // Додатковий зручний оверлоад для Guid
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task Update(T entity);
        Task Delete(T entity);
    }
}
