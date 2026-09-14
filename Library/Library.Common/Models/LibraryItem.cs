using System;

namespace Library.Common.Models
{
    // Делегат
    public delegate void ItemStatusChangedHandler(string message);

    public abstract class LibraryItem : IIdentifiable
    {
        // Статичне поле
        private static int _totalItemsCount;

        // Подія
        public event ItemStatusChangedHandler OnStatusChanged;

        // Властивості (від 3-х у класі)
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int PublicationYear { get; set; }
        public bool IsAvailable { get; set; }

        // Статичний конструктор
        static LibraryItem()
        {
            _totalItemsCount = 0;
        }

        // Конструктор
        protected LibraryItem()
        {
            Id = Guid.NewGuid();
            IsAvailable = true;
            _totalItemsCount++;
        }

        // Конструктор з параметрами
        protected LibraryItem(string title, int publicationYear) : this()
        {
            Title = title;
            PublicationYear = publicationYear;
        }

        // Статичний метод
        public static int GetTotalItemsCount()
        {
            return _totalItemsCount;
        }

        // Метод
        public virtual void ChangeAvailability(bool available)
        {
            IsAvailable = available;
            string status = available ? "повернуто до бібліотеки" : "видано читачеві";
            OnStatusChanged?.Invoke($"Одиниця '{Title}' змінила статус: {status}.");
        }
    }
}
