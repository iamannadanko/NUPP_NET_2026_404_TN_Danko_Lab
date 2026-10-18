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

        // Статичний фабричний метод для створення випадкового журналу
        public static Magazine CreateNew(Random? rnd = null)
        {
            rnd ??= Random.Shared;
            var titles = new[] { "Вісник", "Наука і життя", "Пізнайко", "Український журнал", "Журнал" };
            var pubs = new[] { "Наука", "Освіта", "Культура", "Життя" };
            var cats = new[] { "Наука", "Історія", "Туризм", "Мистецтво" };
            string title = titles[rnd.Next(titles.Length)];
            int issue = rnd.Next(1, 200);
            string pub = pubs[rnd.Next(pubs.Length)];
            string cat = cats[rnd.Next(cats.Length)];
            int year = rnd.Next(1990, 2024);
            return new Magazine(title, issue, pub, cat, year);
        }
    }
}
