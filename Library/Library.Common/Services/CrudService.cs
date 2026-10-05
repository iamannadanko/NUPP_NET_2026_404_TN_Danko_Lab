using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Library.Common.Models;

namespace Library.Common.Services
{
    public class CrudService<T> : ICrudService<T> where T : class, IIdentifiable
    {
        private readonly List<T> _items = new();

        public void Create(T element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (_items.Any(x => x.Id == element.Id))
                throw new InvalidOperationException($"Елемент із Id {element.Id} вже існує.");

            _items.Add(element);
        }

        public T Read(Guid id)
        {
            return _items.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<T> ReadAll()
        {
            return _items.ToList();
        }

        public void Update(T element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var existingIndex = _items.FindIndex(x => x.Id == element.Id);
            if (existingIndex == -1)
                throw new KeyNotFoundException($"Елемент із Id {element.Id} не знайдено.");

            _items[existingIndex] = element;
        }

        public void Remove(T element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var item = _items.FirstOrDefault(x => x.Id == element.Id);
            if (item != null)
            {
                _items.Remove(item);
            }
        }

        // Додаткове завдання: Збереження у файл
        public void Save(string filePath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) // підтримка кирилиці
            };
            string json = JsonSerializer.Serialize(_items, options);
            File.WriteAllText(filePath, json);
        }

        // Додаткове завдання: Завантаження із файлу
        public void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл {filePath} не знайдено.");

            string json = File.ReadAllText(filePath);
            var items = JsonSerializer.Deserialize<List<T>>(json);

            _items.Clear();
            if (items != null)
            {
                _items.AddRange(items);
            }
        }
    }
}
