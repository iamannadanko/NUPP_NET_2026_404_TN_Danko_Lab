using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Library.Infrastructure.Models;

namespace Library.Infrastructure
{
    // Контекст бази даних Entity Framework Core
    public class LibraryContext : DbContext
    {
        public DbSet<LibraryItemModel> LibraryItems => Set<LibraryItemModel>();
        public DbSet<BookModel> Books => Set<BookModel>();
        public DbSet<MagazineModel> Magazines => Set<MagazineModel>();
        public DbSet<PublisherModel> Publishers => Set<PublisherModel>();
        public DbSet<ReaderModel> Readers => Set<ReaderModel>();
        public DbSet<LibraryCardModel> LibraryCards => Set<LibraryCardModel>();
        public DbSet<BookRentalModel> BookRentals => Set<BookRentalModel>();

        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {
        }

        // Конструктор за замовчуванням для підтримки дизайн-тайму міграцій
        public LibraryContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string? connStr = Environment.GetEnvironmentVariable("SUPABASE_CONNECTION_STRING");

                if (string.IsNullOrEmpty(connStr))
                {
                    try
                    {
                        var jsonPath = System.IO.Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                        var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                            .AddJsonFile(jsonPath, optional: true)
                            .Build();
                        connStr = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString(config, "Supabase");
                    }
                    catch
                    {
                        // Пропускаємо, якщо файл конфігурації ще не створено
                    }
                }

                if (string.IsNullOrEmpty(connStr))
                {
                    connStr = "Host=localhost;Database=library_db;Username=postgres;Password=postgres";
                }
                optionsBuilder.UseNpgsql(connStr);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. TPT (Table-per-Type) наслідування:
            // Базовий клас LibraryItemModel та класи-нащадки BookModel і MagazineModel зберігаються в окремих таблицях
            modelBuilder.Entity<LibraryItemModel>()
                .UseTptMappingStrategy()
                .ToTable("LibraryItems");

            modelBuilder.Entity<BookModel>()
                .ToTable("Books");

            modelBuilder.Entity<MagazineModel>()
                .ToTable("Magazines");

            // 2. Зв'язок 1:1 (Один-до-одного):
            // Один ReaderModel має один LibraryCardModel (зовнішній ключ ReaderId у таблиці LibraryCards)
            modelBuilder.Entity<ReaderModel>()
                .HasOne(r => r.LibraryCard)
                .WithOne(c => c.Reader)
                .HasForeignKey<LibraryCardModel>(c => c.ReaderId)
                .OnDelete(DeleteBehavior.Cascade);

            // 3. Зв'язок 1:N (Один-до-багатьох):
            // Одне видавництво PublisherModel випускає багато журналів MagazineModel
            modelBuilder.Entity<PublisherModel>()
                .HasMany(p => p.Magazines)
                .WithOne(m => m.Publisher)
                .HasForeignKey(m => m.PublisherId)
                .OnDelete(DeleteBehavior.Cascade);

            // 4. Зв'язок N:M (Багато-до-багатьох) для додаткових балів:
            // ReaderModel та BookModel зв'язані через проміжну таблицю BookRentalModel
            modelBuilder.Entity<BookRentalModel>()
                .HasKey(br => br.Id);

            modelBuilder.Entity<BookRentalModel>()
                .HasOne(br => br.Reader)
                .WithMany(r => r.Rentals)
                .HasForeignKey(br => br.ReaderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookRentalModel>()
                .HasOne(br => br.Book)
                .WithMany(b => b.Rentals)
                .HasForeignKey(br => br.BookId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
