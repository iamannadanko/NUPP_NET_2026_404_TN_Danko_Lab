using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Library.Infrastructure;
using Library.Infrastructure.Models;
using Library.Infrastructure.Repositories;
using Library.Infrastructure.Services;

namespace Library.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=================================================================");
            Console.WriteLine("       ЛАБОРАТОРНА РОБОТА №3: Entity Framework Core & Supabase   ");
            Console.WriteLine("=================================================================\n");

            // 1. Ініціалізація контексту бази даних
            using var context = new LibraryContext();
            Console.WriteLine("-> Підключення до хмарної бази даних Supabase (PostgreSQL)...");

            // 2. Ініціалізація Репозиторіїв (Завдання 4)
            var bookRepo = new Repository<BookModel>(context);
            var publisherRepo = new Repository<PublisherModel>(context);
            var magazineRepo = new Repository<MagazineModel>(context);
            var readerRepo = new Repository<ReaderModel>(context);
            var rentalRepo = new Repository<BookRentalModel>(context);

            // 3. Ініціалізація оновленого асинхронного CRUD сервісу на базі репозиторію (Завдання 5)
            var bookService = new DbCrudServiceAsync<BookModel>(bookRepo);

            Console.WriteLine("-> CRUD-сервіс успішно підключено до репозиторію бази даних!\n");

            // -------------------------------------------------------------
            // ДЕМОНСТРАЦІЯ ЗВ'ЯЗКУ 1:N (Один-до-багатьох): Publisher -> Magazines
            // -------------------------------------------------------------
            Console.WriteLine("--- 1. Демонстрація зв'язку 1:N (Видавництво -> Журнали) ---");
            var publisher = new PublisherModel
            {
                Name = "Наукова Думка",
                Address = "м. Київ, вул. Грушевського, 4"
            };
            await publisherRepo.AddAsync(publisher);

            var magazine = new MagazineModel
            {
                Title = "Вісник Академії Наук",
                PublicationYear = 2024,
                IssueNumber = 12,
                Category = "Наука та Технології",
                PublisherId = publisher.Id
            };
            await magazineRepo.AddAsync(magazine);
            Console.WriteLine($"[+] Створено видавництво: \"{publisher.Name}\" (ID: {publisher.Id})");
            Console.WriteLine($"[+] До нього додано журнал: \"{magazine.Title}\" №{magazine.IssueNumber} (Видавництво ID: {magazine.PublisherId})\n");

            // -------------------------------------------------------------
            // ДЕМОНСТРАЦІЯ ЗВ'ЯЗКУ 1:1 (Один-до-одного): Reader <-> LibraryCard
            // -------------------------------------------------------------
            Console.WriteLine("--- 2. Демонстрація зв'язку 1:1 (Читач <-> Читацький квиток) ---");
            var reader = new ReaderModel
            {
                FullName = "Олександр Петренко",
                Email = "oleksandr.petrenko@example.com"
            };
            await readerRepo.AddAsync(reader);

            var card = new LibraryCardModel
            {
                CardNumber = "CARD-2026-0042",
                ReaderId = reader.Id,
                IssueDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddYears(1)
            };
            context.LibraryCards.Add(card);
            await context.SaveChangesAsync();
            Console.WriteLine($"[+] Створено читача: {reader.FullName} ({reader.Email})");
            Console.WriteLine($"[+] Йому видано читацький квиток 1:1: №{card.CardNumber} (ReaderId: {card.ReaderId})\n");

            // -------------------------------------------------------------
            // ДЕМОНСТРАЦІЯ CRUD СЕРВІСУ (Create, Read, Update, Delete) + TPT
            // -------------------------------------------------------------
            Console.WriteLine("--- 3. Демонстрація методів CRUD-сервісу через Репозиторій ---");

            // CreateAsync
            var book1 = new BookModel
            {
                Title = "Тіні забутих предків",
                Author = "Михайло Коцюбинський",
                PublicationYear = 1911,
                ISBN = "978-966-03-8472-1",
                PageCount = 224
            };
            var book2 = new BookModel
            {
                Title = "Кобзар",
                Author = "Тарас Шевченко",
                PublicationYear = 1840,
                ISBN = "978-966-03-1234-5",
                PageCount = 640
            };
            var book3 = new BookModel
            {
                Title = "Тигролови",
                Author = "Іван Багряний",
                PublicationYear = 1944,
                ISBN = "978-966-03-9999-0",
                PageCount = 312
            };

            await bookService.CreateAsync(book1);
            await bookService.CreateAsync(book2);
            await bookService.CreateAsync(book3);
            Console.WriteLine("[+] Через CRUD сервіс успішно додано 3 книги в базу Supabase!");

            // ReadAsync (за Guid)
            var fetchedBook = await bookService.ReadAsync(book1.Id);
            Console.WriteLine($"[?] ReadAsync(Id) знайдено: \"{fetchedBook.Title}\" ({fetchedBook.Author}), сторінок: {fetchedBook.PageCount}");

            // UpdateAsync
            fetchedBook.PageCount = 230; // Оновлюємо кількість сторінок
            await bookService.UpdateAsync(fetchedBook);
            var updatedBook = await bookService.ReadAsync(book1.Id);
            Console.WriteLine($"[*] UpdateAsync: оновлено сторінки до {updatedBook.PageCount} (Успішно!)");

            // -------------------------------------------------------------
            // ДЕМОНСТРАЦІЯ ЗВ'ЯЗКУ N:M (Багато-до-багатьох): Reader <-> BookRental <-> Book
            // -------------------------------------------------------------
            Console.WriteLine("\n--- 4. Демонстрація зв'язку N:M (Оренда книги читачем) ---");
            var rental = new BookRentalModel
            {
                ReaderId = reader.Id,
                BookId = book1.Id,
                RentalDate = DateTime.UtcNow
            };
            await rentalRepo.AddAsync(rental);
            Console.WriteLine($"[+] Читач {reader.FullName} орендував книгу \"{book1.Title}\" (Запис оренди ID: {rental.Id})\n");

            // -------------------------------------------------------------
            // ДЕМОНСТРАЦІЯ ПАГІНАЦІЇ (ReadAllAsync з page і amount)
            // -------------------------------------------------------------
            Console.WriteLine("--- 5. Демонстрація пагінації з бази даних (Сторінка 1, розмір 2) ---");
            var page1 = (await bookService.ReadAllAsync(page: 1, amount: 2)).ToList();
            int index = 1;
            foreach (var b in page1)
            {
                Console.WriteLine($"   {index++}. \"{b.Title}\" - {b.Author} ({b.PublicationYear}) | ISBN: {b.ISBN}");
            }

            // -------------------------------------------------------------
            // ДЕМОНСТРАЦІЯ TPT (Table-per-Type) Наслідування
            // -------------------------------------------------------------
            Console.WriteLine("\n--- 6. Перевірка TPT-наслідування (Базова таблиця LibraryItems) ---");
            var totalLibraryItems = await context.LibraryItems.CountAsync();
            var totalBooks = await context.Books.CountAsync();
            var totalMagazines = await context.Magazines.CountAsync();
            Console.WriteLine($"-> Загальна кількість сутностей у базовій таблиці LibraryItems: {totalLibraryItems}");
            Console.WriteLine($"-> З них книг (таблиця Books): {totalBooks}");
            Console.WriteLine($"-> З них журналів (таблиця Magazines): {totalMagazines}");

            Console.WriteLine("\n=================================================================");
            Console.WriteLine("        Усі операції успішно виконано в базі даних Supabase!     ");
            Console.WriteLine("=================================================================");

            Console.WriteLine("\nНатисніть будь-яку клавішу для завершення...");
            if (!Console.IsInputRedirected)
            {
                Console.ReadKey();
            }
        }
    }
}
