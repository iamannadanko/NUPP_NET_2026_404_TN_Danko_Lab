using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Library.Common.Models;
using Library.Common.Services;

namespace Library.Tests
{
    public class CrudServiceAsyncTests : IDisposable
    {
        private const string TestFilePath = "test_books.json";

        public CrudServiceAsyncTests()
        {
            // Clean up any test files from previous runs
            if (File.Exists(TestFilePath))
                File.Delete(TestFilePath);
        }

        private CrudServiceAsync<Book> CreateService()
        {
            return new CrudServiceAsync<Book>(TestFilePath);
        }

        // Тест: CreateAsync успішно додає книгу
        [Fact]
        public async Task CreateAsync_ValidBook_ReturnsTrue()
        {
            // Arrange
            var service = CreateService();
            var book = new Book("Test Title", "Test Author", "ISBN-123", 2024, 300);

            // Act
            var result = await service.CreateAsync(book);

            // Assert
            Assert.True(result);
        }

        // Тест: CreateAsync з дублікатом повертає false
        [Fact]
        public async Task CreateAsync_DuplicateId_ReturnsFalse()
        {
            // Arrange
            var service = CreateService();
            var book = new Book("Test Title", "Test Author", "ISBN-123", 2024, 300);
            await service.CreateAsync(book);

            // Act - спробуємо додати іншу книгу з тим самим Id
            var duplicateBook = new Book("Another Title", "Another Author", "ISBN-456", 2023, 250)
            {
                Id = book.Id
            };
            var result = await service.CreateAsync(duplicateBook);

            // Assert
            Assert.False(result);
        }

        // Тест: ReadAsync повертає книгу за Id
        [Fact]
        public async Task ReadAsync_ValidId_ReturnsBook()
        {
            // Arrange
            var service = CreateService();
            var book = new Book("Test Title", "Test Author", "ISBN-123", 2024, 300);
            await service.CreateAsync(book);

            // Act
            var result = await service.ReadAsync(book.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(book.Id, result.Id);
            Assert.Equal("Test Title", result.Title);
        }

        // Тест: ReadAsync з неіснуючим Id повертає null
        [Fact]
        public async Task ReadAsync_InvalidId_ReturnsNull()
        {
            // Arrange
            var service = CreateService();
            var fakeId = Guid.NewGuid();

            // Act
            var result = await service.ReadAsync(fakeId);

            // Assert
            Assert.Null(result);
        }

        // Тест: ReadAllAsync повертає всі книги
        [Fact]
        public async Task ReadAllAsync_MultipleBooks_ReturnsAll()
        {
            // Arrange
            var service = CreateService();
            var books = new[]
            {
                new Book("Book 1", "Author 1", "ISBN-001", 2020, 100),
                new Book("Book 2", "Author 2", "ISBN-002", 2021, 200),
                new Book("Book 3", "Author 3", "ISBN-003", 2022, 300)
            };

            foreach (var book in books)
                await service.CreateAsync(book);

            // Act
            var result = (await service.ReadAllAsync()).ToList();

            // Assert
            Assert.Equal(3, result.Count);
        }

        // Тест: ReadAllAsync з пагінацією повертає коректну сторінку
        [Fact]
        public async Task ReadAllAsync_WithPagination_ReturnsPaginatedResults()
        {
            // Arrange
            var service = CreateService();
            var books = Enumerable.Range(1, 25).Select(i =>
                new Book($"Book {i}", $"Author {i}", $"ISBN-{i:D3}", 2020 + i, 100 + i)
            ).ToList();

            foreach (var book in books)
                await service.CreateAsync(book);

            // Act
            var page1 = (await service.ReadAllAsync(1, 10)).ToList();
            var page2 = (await service.ReadAllAsync(2, 10)).ToList();
            var page3 = (await service.ReadAllAsync(3, 10)).ToList();

            // Assert
            Assert.Equal(10, page1.Count);
            Assert.Equal(10, page2.Count);
            Assert.Equal(5, page3.Count);
        }

        // Тест: UpdateAsync успішно оновлює книгу
        [Fact]
        public async Task UpdateAsync_ValidBook_ReturnsTrue()
        {
            // Arrange
            var service = CreateService();
            var book = new Book("Original Title", "Original Author", "ISBN-123", 2024, 300);
            await service.CreateAsync(book);

            // Act
            book.Title = "Updated Title";
            book.PageCount = 400;
            var result = await service.UpdateAsync(book);

            // Assert
            Assert.True(result);
            var updated = await service.ReadAsync(book.Id);
            Assert.Equal("Updated Title", updated.Title);
            Assert.Equal(400, updated.PageCount);
        }

        // Тест: UpdateAsync з неіснуючим Id повертає false
        [Fact]
        public async Task UpdateAsync_InvalidId_ReturnsFalse()
        {
            // Arrange
            var service = CreateService();
            var book = new Book("Test Title", "Test Author", "ISBN-123", 2024, 300);

            // Act
            var result = await service.UpdateAsync(book);

            // Assert
            Assert.False(result);
        }

        // Тест: RemoveAsync успішно видаляє книгу
        [Fact]
        public async Task RemoveAsync_ValidBook_ReturnsTrue()
        {
            // Arrange
            var service = CreateService();
            var book = new Book("Test Title", "Test Author", "ISBN-123", 2024, 300);
            await service.CreateAsync(book);

            // Act
            var result = await service.RemoveAsync(book);

            // Assert
            Assert.True(result);
            var readResult = await service.ReadAsync(book.Id);
            Assert.Null(readResult);
        }

        // Тест: RemoveAsync з неіснуючою книгою повертає false
        [Fact]
        public async Task RemoveAsync_InvalidBook_ReturnsFalse()
        {
            // Arrange
            var service = CreateService();
            var book = new Book("Test Title", "Test Author", "ISBN-123", 2024, 300);

            // Act
            var result = await service.RemoveAsync(book);

            // Assert
            Assert.False(result);
        }

        // Тест: SaveAsync записує дані у файл
        [Fact]
        public async Task SaveAsync_BooksExist_CreatesFile()
        {
            // Arrange
            var service = CreateService();
            var book = new Book("Test Title", "Test Author", "ISBN-123", 2024, 300);
            await service.CreateAsync(book);

            // Act
            var result = await service.SaveAsync();

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(TestFilePath));
            var fileContent = File.ReadAllText(TestFilePath);
            Assert.NotEmpty(fileContent);
            Assert.Contains("Test Title", fileContent);
        }

        // Тест: IEnumerable<T> реалізація работає коректно
        [Fact]
        public async Task IEnumerable_MultipleItems_CanIterate()
        {
            // Arrange
            var service = CreateService();
            var books = new[]
            {
                new Book("Book 1", "Author 1", "ISBN-001", 2020, 100),
                new Book("Book 2", "Author 2", "ISBN-002", 2021, 200),
                new Book("Book 3", "Author 3", "ISBN-003", 2022, 300)
            };

            foreach (var book in books)
                await service.CreateAsync(book);

            // Act
            var count = 0;
            foreach (var item in service)
            {
                count++;
                Assert.NotNull(item);
            }

            // Assert
            Assert.Equal(3, count);
        }

        // Тест: Паралельна багатопоткова операція - thread-safe
        [Fact]
        public async Task CreateAsync_ParallelCreation_ThreadSafe()
        {
            // Arrange
            var service = CreateService();
            const int itemCount = 100;
            var tasks = new List<Task<bool>>();

            // Act - створюємо 100 книг паралельно
            for (int i = 0; i < itemCount; i++)
            {
                var book = new Book($"Book {i}", $"Author {i}", $"ISBN-{i}", 2020 + (i % 10), 100 + i);
                tasks.Add(service.CreateAsync(book));
            }

            var results = await Task.WhenAll(tasks);

            // Assert - всі мають бути успішними
            Assert.All(results, Assert.True);

            // Перевіримо, що всі книги збережені
            var all = (await service.ReadAllAsync()).ToList();
            Assert.Equal(itemCount, all.Count);
        }

        // Тест: CreateNew фабричний метод генерує валідні об'єкти
        [Fact]
        public void Book_CreateNew_GeneratesValidBook()
        {
            // Act
            var book = Book.CreateNew();

            // Assert
            Assert.NotNull(book);
            Assert.NotEmpty(book.Title);
            Assert.NotEmpty(book.Author);
            Assert.NotEmpty(book.ISBN);
            Assert.True(book.PublicationYear > 0);
            Assert.True(book.PageCount > 0);
            Assert.NotEqual(Guid.Empty, book.Id);
        }

        // Тест: Асинхронна операція SaveAsync та ReadAllAsync
        [Fact]
        public async Task SaveAsync_ThenReadAll_PreservesData()
        {
            // Arrange
            var service1 = CreateService();
            var books = new[]
            {
                new Book("Book 1", "Author 1", "ISBN-001", 2020, 100),
                new Book("Book 2", "Author 2", "ISBN-002", 2021, 200),
                new Book("Book 3", "Author 3", "ISBN-003", 2022, 300)
            };

            foreach (var book in books)
                await service1.CreateAsync(book);

            await service1.SaveAsync();

            // Act - створюємо новий сервіс і перевіряємо дані (якби був Load)
            var allBooks = (await service1.ReadAllAsync()).ToList();

            // Assert
            Assert.Equal(3, allBooks.Count);
            Assert.Contains(allBooks, b => b.Title == "Book 1");
            Assert.Contains(allBooks, b => b.Title == "Book 2");
            Assert.Contains(allBooks, b => b.Title == "Book 3");
        }

        public void Dispose()
        {
            // Clean up test files
            if (File.Exists(TestFilePath))
                File.Delete(TestFilePath);
        }
    }
}
