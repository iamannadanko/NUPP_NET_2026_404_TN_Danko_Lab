using System;

namespace Library.Common.Extensions
{
    public static class LibraryExtensions
    {
        // Метод розширення для швидкого опису елемента бібліотеки
        public static string GetShortDescription(this Models.LibraryItem item)
        {
            if (item == null) return string.Empty;
            return $"{item.Title} ({item.PublicationYear}) - ID: {item.Id}";
        }
    }
}
