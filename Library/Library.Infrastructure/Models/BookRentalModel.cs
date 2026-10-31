using System;

namespace Library.Infrastructure.Models
{
    // Таблиця зв'язку для Many-to-Many (N:M) між ReaderModel та BookModel
    public class BookRentalModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ReaderId { get; set; }
        public ReaderModel Reader { get; set; } = null!;

        public Guid BookId { get; set; }
        public BookModel Book { get; set; } = null!;

        public DateTime RentalDate { get; set; } = DateTime.UtcNow;
        public DateTime? ReturnDate { get; set; }
    }
}
