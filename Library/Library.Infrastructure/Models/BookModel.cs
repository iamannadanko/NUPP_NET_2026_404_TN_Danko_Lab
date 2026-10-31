using System;
using System.Collections.Generic;

namespace Library.Infrastructure.Models
{
    // Модель книги, яка наслідується від LibraryItemModel (TPT наслідування)
    public class BookModel : LibraryItemModel
    {
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PageCount { get; set; }

        // Зв'язок багато-до-багатьох (N:M) із ReaderModel через BookRentalModel
        public ICollection<BookRentalModel> Rentals { get; set; } = new List<BookRentalModel>();
    }
}
