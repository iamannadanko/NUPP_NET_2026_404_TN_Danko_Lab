using System;

namespace Library.Infrastructure.Models
{
    // Модель читацького квитка для зв'язку 1:1 із ReaderModel
    public class LibraryCardModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CardNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; } = DateTime.UtcNow;
        public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddYears(1);

        // Зв'язок 1:1: квиток належить рівно одному читачеві
        public Guid ReaderId { get; set; }
        public ReaderModel Reader { get; set; } = null!;
    }
}
