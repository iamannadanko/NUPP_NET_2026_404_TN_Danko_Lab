using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using Library.Common.Models;

namespace Library.Common.Services
{
    public class CrudServiceAsync<T> : ICrudServiceAsync<T> where T : class, IIdentifiable
    {
        private readonly ConcurrentDictionary<Guid, T> _items = new();
        private readonly SemaphoreSlim _fileSemaphore = new(1, 1);

        public string FilePath { get; set; }

        public CrudServiceAsync(string filePath = "crud_data.json")
        {
            FilePath = filePath;
        }

        public Task<bool> CreateAsync(T element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element)); //додавання елемента
            bool added = _items.TryAdd(element.Id, element);
            return Task.FromResult(added);
        }

        public Task<T> ReadAsync(Guid id)
        {
            _items.TryGetValue(id, out var item); //шукання за ід 
            return Task.FromResult(item!);
        }

        public Task<IEnumerable<T>> ReadAllAsync()
        {
            var snapshot = _items.Values.ToList();
            return Task.FromResult<IEnumerable<T>>(snapshot);
        }

        public Task<IEnumerable<T>> ReadAllAsync(int page, int amount) //пагінація
        {
            if (page < 1) page = 1;
            if (amount < 1) amount = 10;
            var snapshot = _items.Values.Skip((page - 1) * amount).Take(amount).ToList();
            return Task.FromResult<IEnumerable<T>>(snapshot);
        }

        public Task<bool> UpdateAsync(T element) //атомарне оновлення
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            while (_items.TryGetValue(element.Id, out var existing))
            {
                if (_items.TryUpdate(element.Id, element, existing))
                    return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> RemoveAsync(T element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            return Task.FromResult(_items.TryRemove(element.Id, out _));
        }

        public async Task<bool> SaveAsync()
        {
            await _fileSemaphore.WaitAsync();// Чекаємо своєї черги на доступ до файлу
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true, //форматування
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) //кирилиця
                };
                var list = _items.Values.ToList(); //знімок
                string json = JsonSerializer.Serialize(list, options); //асинхронна операція запису на жорсткий диск 
                await File.WriteAllTextAsync(FilePath, json);
                return true;
            }
            finally
            {
                _fileSemaphore.Release(); //звільняємо семафор у блоці finally!
            }
        }

        // IEnumerable<T> implementation
        public IEnumerator<T> GetEnumerator()
        {
            return _items.Values.ToList().GetEnumerator(); // Повертаємо ітератор по знімку даних
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
