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
        private readonly ReaderWriterLockSlim _rwLock = new();
        private readonly SemaphoreSlim _fileSemaphore = new(1, 1);
        private readonly string _filePath;

        public CrudServiceAsync(string filePath = "crud_data.json")
        {
            _filePath = filePath;
        }

        public async Task<bool> CreateAsync(T element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            bool added = _items.TryAdd(element.Id, element);
            return await Task.FromResult(added);
        }

        public async Task<T> ReadAsync(Guid id)
        {
            _items.TryGetValue(id, out var item);
            return await Task.FromResult(item);
        }

        public async Task<IEnumerable<T>> ReadAllAsync()
        {
            var snapshot = _items.Values.ToList();
            return await Task.FromResult(snapshot.AsEnumerable());
        }

        public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
        {
            if (page < 1) page = 1;
            if (amount < 1) amount = 10;
            var snapshot = _items.Values.Skip((page - 1) * amount).Take(amount).ToList();
            return await Task.FromResult(snapshot.AsEnumerable());
        }

        public async Task<bool> UpdateAsync(T element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            if (!_items.ContainsKey(element.Id)) return await Task.FromResult(false);
            _items[element.Id] = element;
            return await Task.FromResult(true);
        }

        public async Task<bool> RemoveAsync(T element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            return await Task.FromResult(_items.TryRemove(element.Id, out _));
        }

        public async Task<bool> SaveAsync()
        {
            await _fileSemaphore.WaitAsync();
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                };
                var list = _items.Values.ToList();
                string json = JsonSerializer.Serialize(list, options);
                await File.WriteAllTextAsync(_filePath, json);
                return true;
            }
            finally
            {
                _fileSemaphore.Release();
            }
        }

        // IEnumerable<T> implementation
        public IEnumerator<T> GetEnumerator()
        {
            return _items.Values.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
