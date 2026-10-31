using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Common.Services;
using Library.Infrastructure.Repositories;

namespace Library.Infrastructure.Services
{
    // Асинхронний CRUD сервіс, який використовує шаблон Репозиторій для доступу до бази даних (Завдання 5)
    public class DbCrudServiceAsync<T> : ICrudServiceAsync<T> where T : class
    {
        private readonly IRepository<T> _repository;

        public DbCrudServiceAsync(IRepository<T> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<bool> CreateAsync(T element)
        {
            if (element == null) return false;
            try
            {
                await _repository.AddAsync(element);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<T> ReadAsync(Guid id)
        {
            var result = await _repository.GetByIdAsync(id);
            return result!;
        }

        public async Task<IEnumerable<T>> ReadAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
        {
            if (page < 1) page = 1;
            if (amount < 1) amount = 10;
            var all = await _repository.GetAllAsync();
            return all.Skip((page - 1) * amount).Take(amount).ToList();
        }

        public async Task<bool> UpdateAsync(T element)
        {
            if (element == null) return false;
            try
            {
                await _repository.Update(element);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveAsync(T element)
        {
            if (element == null) return false;
            try
            {
                await _repository.Delete(element);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task<bool> SaveAsync()
        {
            // Усі зміни автоматично фіксуються в БД через репозиторій
            return Task.FromResult(true);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _repository.GetAllAsync().GetAwaiter().GetResult().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
