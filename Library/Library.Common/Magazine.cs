using System;

namespace Library.Common.Models
{
    public class Magazine : LibraryItem
    {
        // Властивості
        public int IssueNumber { get; set; }
        public string Publisher { get; set; }
        public string Category { get; set; }

        // Конструктор
        public Magazine() : base()
        {
        }

        // Конструктор з параметрами
        public Magazine(string title, int issueNumber, string publisher, string category, int year)
            : base(title, year)
        {
            IssueNumber = issueNumber;
            Publisher = publisher;
            Category = category;
        }

        // Метод
        public override string ToString()
        {
            return $"[Журнал] ID: {Id} | \"{Title}\" №{IssueNumber} ({PublicationYear}), Видавництво: {Publisher}, Категорія: {Category}, Доступний: {IsAvailable}";
        }
    }
}