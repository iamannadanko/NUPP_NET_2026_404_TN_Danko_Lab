using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Common.Services
{
    // Дженерік-інтерфейс для асинхронного CRUD сервісу.
    // Наслідує IEnumerable<T>, що дозволяє перебирати елементи сервісу у циклі foreach або через LINQ.
    public interface ICrudServiceAsync<T> : IEnumerable<T>
    {
        // Асинхронне додавання нового об'єкта (повертає true, якщо елемент успішно додано)
        public Task<bool> CreateAsync(T element);

        // Асинхронне читання одного об'єкта за його унікальним ідентифікатором Guid
        public Task<T> ReadAsync(Guid id);

        // Асинхронне читання всіх об'єктів колекції
        public Task<IEnumerable<T>> ReadAllAsync();

        // Асинхронне читання даних із пагінацією (посторінковий вивід: номер сторінки page, кількість на сторінці amount)
        public Task<IEnumerable<T>> ReadAllAsync(int page, int amount);

        // Асинхронне оновлення існуючого об'єкта
        public Task<bool> UpdateAsync(T element);

        // Асинхронне видалення об'єкта
        public Task<bool> RemoveAsync(T element);

        // Асинхронне збереження колекції у файл на диску
        public Task<bool> SaveAsync();
    }
}
