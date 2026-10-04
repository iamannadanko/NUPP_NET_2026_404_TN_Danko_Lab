using System;
using System.Collections.Generic;

namespace Library.Common.Models
{
    // Клас читача бібліотеки
    public class Reader : IIdentifiable
    {
        // Властивості
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public List<Guid> BorrowedItemIds { get; set; }

        // Конструктор
        public Reader()
        {
            Id = Guid.NewGuid();
            BorrowedItemIds = new List<Guid>();
        }

        public Reader(string fullName, string email) : this()
        {
            FullName = fullName;
            Email = email;
        }

        public override string ToString()
        {
            return $"[Читач] ID: {Id} | {FullName} ({Email}) - Запозичень: {BorrowedItemIds?.Count ?? 0}";
        }

        // Статичний фабричний метод для створення випадкового читача
        public static Reader CreateNew(Random? rnd = null)
        {
            rnd ??= new Random();
            var names = new[] { "Олександр", "Марія", "Іван", "Катерина", "Андрій", "Ірина" };
            var surnames = new[] { "Петренко", "Іванов", "Шевченко", "Коваленко", "Бондар" };
            string full = $"{names[rnd.Next(names.Length)]} {surnames[rnd.Next(surnames.Length)]}";
            string email = full.Replace(' ', '.').ToLower() + $"{rnd.Next(1,100)}@example.com";
            return new Reader(full, email);
        }
    }
}
