using System;
using System.Text;
using Library.Common.Extensions;
using Library.Common.Models;
using Library.Common.Services;

namespace Library.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=== ДЕМОНСТРАЦІЯ РОБОТИ CRUD СЕРВІСУ ТА МОДЕЛІ БІБЛІОТЕКИ ===\n");

            ICrudService<Book> bookService = new CrudService<Book>();

            // 1. Створення об'єктів
            var book1 = new Book("Кобзар", "Тарас Шевченко", "978-966-03-8000-1", 1840, 280);
            var book2 = new Book("Тигролови", "Іван Багряний", "978-966-03-8001-2", 1944, 320);
            var book3 = new Book("Тіні забутих предків", "Михайло Коцюбинський", "978-966-03-8002-3", 1911, 190);

            // Підписка на подію
            book1.OnStatusChanged += message => Console.WriteLine($"[ПОДІЯ]: {message}");
            book2.OnStatusChanged += message => Console.WriteLine($"[ПОДІЯ]: {message}");

            // Демонстрація методу розширення
            Console.WriteLine("--- Метод розширення ---");
            Console.WriteLine(book1.GetShortDescription());
            Console.WriteLine();

            // Демонстрація статичного методу і поля
            Console.WriteLine($"Загальна кількість створених елементів у системі (статичне поле): {LibraryItem.GetTotalItemsCount()}\n");

            // 2. CREATE
            Console.WriteLine("--- 1. Додавання книг до сервісу (Create) ---");
            bookService.Create(book1);
            bookService.Create(book2);
            bookService.Create(book3);
            Console.WriteLine("Книги успішно додані.");

            // 3. READ ALL
            Console.WriteLine("\n--- 2. Перегляд усіх книг (ReadAll) ---");
            foreach (var item in bookService.ReadAll())
            {
                Console.WriteLine(item);
            }

            // 4. READ BY ID
            Console.WriteLine($"\n--- 3. Отримання книги за ID (Read): {book2.Id} ---");
            var foundBook = bookService.Read(book2.Id);
            Console.WriteLine(foundBook);

            // Демонстрація події через метод екземпляра
            Console.WriteLine("\n--- Виклик методу зі зміною статусу (тригер події) ---");
            book1.ChangeAvailability(false);

            // 5. UPDATE
            Console.WriteLine("\n--- 4. Оновлення книги (Update) ---");
            foundBook.PageCount = 350; // Змінили кількість сторінок
            bookService.Update(foundBook);
            Console.WriteLine($"Оновлена книга: {bookService.Read(foundBook.Id)}");

            // 6. REMOVE
            Console.WriteLine($"\n--- 5. Видалення книги (Remove): {book3.Title} ---");
            bookService.Remove(book3);
            Console.WriteLine("Залишились у списку після видалення:");
            foreach (var item in bookService.ReadAll())
            {
                Console.WriteLine(item);
            }

            // 7. ДОДАТКОВЕ ЗАВДАННЯ: Save & Load
            Console.WriteLine("\n--- 6. Додаткове завдання: Збереження та завантаження з файлу ---");
            string filePath = "library_books.json";

            bookService.Save(filePath);
            Console.WriteLine($"Дані сервісу успішно збережено у файл '{filePath}'.");

            // Створюємо новий порожній сервіс і вантажимо з файлу
            ICrudService<Book> loadedService = new CrudService<Book>();
            loadedService.Load(filePath);

            Console.WriteLine("Дані, зчитані з файлу в новий екземпляр сервісу:");
            foreach (var item in loadedService.ReadAll())
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }
    }
}