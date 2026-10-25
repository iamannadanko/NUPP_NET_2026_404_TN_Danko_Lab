using System;
using System.Collections.Generic;

namespace Library.Infrastructure.Models
{
    // Модель видавництва для демонстрації зв'язку один-до-багатьох (1:N)
    public class PublisherModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        // Зв'язок 1:N — одне видавництво випускає багато журналів
        public ICollection<MagazineModel> Magazines { get; set; } = new List<MagazineModel>();
    }
}
