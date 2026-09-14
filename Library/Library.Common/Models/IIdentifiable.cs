using System;

namespace Library.Common.Models
{
    // Інтерфейс, що гарантує наявність Id у моделях
    public interface IIdentifiable
    {
        Guid Id { get; set; }
    }
}
