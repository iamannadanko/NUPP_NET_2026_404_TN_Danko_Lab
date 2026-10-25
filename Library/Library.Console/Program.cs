using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Library.Common.Extensions;
using Library.Common.Models;
using Library.Common.Services;

namespace Library.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=== LAB2: Async CRUD, Parallel.ForEachAsync, LINQ stats ===\n");

            const int count = 2000; // number of objects to create
            var bookService = new CrudServiceAsync<Book>("library_books_async.json");

            // Synchronization primitives examples
            var statsLock = new object(); // lock об'єкт-замок для критичної секції (взаємне виключення в пам'яті)
            var semaphore = new SemaphoreSlim(Environment.ProcessorCount); // обмежує кількість потоків, які можуть одночасно виконувати операцію
            var saveCompleted = new AutoResetEvent(false); //сигналізатор(світлофор): один потік спить, поки інший не подасть сигнал Set()

            int created = 0;

            // Повноцінна асинхронна паралельність з Parallel.ForEachAsync (.NET 6+)
            Console.WriteLine($"Starting parallel async creation of {count} books...");

            var range = Enumerable.Range(0, count);

            // Parallel.ForEachAsync - сучасний варіант для асинхронного паралелізму
            await Parallel.ForEachAsync(range, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, 
                async (i, cancellationToken) =>
                {
                    // Демонстрація SemaphoreSlim для контролю конкурентності
                    await semaphore.WaitAsync(cancellationToken);
                    try
                    {
                        var book = Book.CreateNew();
                        await bookService.CreateAsync(book);

                        // Демонстрація lock для безпечного оновлення счету
                        lock (statsLock)
                        {
                            created++;
                            if (created % 500 == 0)
                            {
                                Console.WriteLine($"Created: {created}");
                            }
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

            Console.WriteLine($"Finished creating {created} books.\n");

            // Читання снимку і обчислення статистики через LINQ
            var all = (await bookService.ReadAllAsync()).ToList();
            if (all.Any())
            {
                // Демонстрація LINQ для min/max/average
                var minPages = all.Min(b => b.PageCount);
                var maxPages = all.Max(b => b.PageCount);
                var avgPages = all.Average(b => b.PageCount);

                var minYear = all.Min(b => b.PublicationYear);
                var maxYear = all.Max(b => b.PublicationYear);
                var avgYear = all.Average(b => b.PublicationYear);

                Console.WriteLine("--- Statistics for generated books (LINQ) ---");
                Console.WriteLine($"Pages: min={minPages}, max={maxPages}, avg={avgPages:F2}");
                Console.WriteLine($"Year:  min={minYear}, max={maxYear}, avg={avgYear:F2}");
                Console.WriteLine($"Total books: {all.Count()}\n");
            }

            // Асинхронне збереження в фоні з сигналізацією через AutoResetEvent
            Console.WriteLine("Saving collection to file asynchronously...");
            _ = Task.Run(async () =>
            {
                await bookService.SaveAsync();
                // Демонстрація AutoResetEvent - сигнал після завершення
                saveCompleted.Set();
            });

            // Очікування сигналу AutoResetEvent (демонстрація синхронізаційного примітиву)
            saveCompleted.WaitOne();
            Console.WriteLine("Save completed and signaled via AutoResetEvent.\n");

            Console.WriteLine("Sample items:");
            foreach (var item in all.Take(5))
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("\nPress any key to exit...");
            if (!Console.IsInputRedirected)
            {
                Console.ReadKey();
            }
        }
    }
}
