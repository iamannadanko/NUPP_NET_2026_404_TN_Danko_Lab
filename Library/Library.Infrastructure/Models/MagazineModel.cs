using System;

namespace Library.Infrastructure.Models
{
    // Модель журналу, яка наслідується від LibraryItemModel (TPT наслідування)
    public class MagazineModel : LibraryItemModel
    {
        public int IssueNumber { get; set; }
        public string Category { get; set; } = string.Empty;

        // Зв'язок 1:N — журнал належить одному видавництву (PublisherModel)
        public Guid PublisherId { get; set; }
        public PublisherModel Publisher { get; set; } = null!;
    }
}
