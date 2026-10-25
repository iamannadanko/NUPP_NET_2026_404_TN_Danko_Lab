using System;
using System.Collections.Generic;

namespace Library.Infrastructure.Models
{
    // Модель читача
    public class ReaderModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Зв'язок 1:1 — у читача є один читацький квиток
        public LibraryCardModel? LibraryCard { get; set; }

        // Зв'язок N:M — читач може орендувати багато книг
        public ICollection<BookRentalModel> Rentals { get; set; } = new List<BookRentalModel>();
    }
}
