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

        // Статичний фабричний метод для створення випадкової книги
        public static Book CreateNew(Random? rnd = null)
        {
            rnd ??= new Random();
            var titles = new[] { "Кобзар", "Тигролови", "Тіні забутих предків", "Лісова пісня", "Енеїда" };
            var authors = new[] { "Тарас Шевченко", "Іван Багряний", "Михайло Коцюбинський", "Леся Українка", "Іван Котляревський" };
            string title = titles[rnd.Next(titles.Length)];
            string author = authors[rnd.Next(authors.Length)];
            string isbn = $"978-966-0{rnd.Next(100,999)}-{rnd.Next(1000,9999)}-{rnd.Next(1,9)}";
            int year = rnd.Next(1800, 2024);
            int pages = rnd.Next(50, 800);
            return new Book(title, author, isbn, year, pages);
        }
    }
}
