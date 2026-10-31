using System;

namespace Library.Infrastructure.Models
{
    // Базова сутність для TPT (Table-per-Type) наслідування
    public abstract class LibraryItemModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
