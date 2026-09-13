using System;

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
    }
}
