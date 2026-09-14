using System;

namespace Library.Common.Models
{
    public class Book : LibraryItem
    {
        // Властивості
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int PageCount { get; set; }

        // Конструктор
        public Book() : base()
        {
        }

        // Конструктор з параметрами
        public Book(string title, string author, string isbn, int year, int pages)
            : base(title, year)
        {
            Author = author;
            ISBN = isbn;
            PageCount = pages;
        }

        // Метод
        public override string ToString()
        {
            return $"[Книга] ID: {Id} | \"{Title}\" - {Author} ({PublicationYear}), {PageCount} стор., Доступна: {IsAvailable}";
        }
    }
}
