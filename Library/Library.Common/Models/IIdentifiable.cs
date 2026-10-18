using System;

namespace Library.Common.Models
{
    // наявність Id у моделях
    public interface IIdentifiable
    {
        Guid Id { get; set; }
    }
}
