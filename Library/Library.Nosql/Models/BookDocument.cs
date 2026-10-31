using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Library.Nosql.Models
{
    // Документоорієнтована NoSQL-модель для збереження в MongoDB
    public class BookDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PageCount { get; set; }
        public int PublicationYear { get; set; }
        public List<string> Genres { get; set; } = new(); // NoSQL дозволяє вкладені масиви без окремих таблиць!
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
