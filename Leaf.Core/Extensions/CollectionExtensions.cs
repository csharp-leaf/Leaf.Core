using System.Collections.Generic;

namespace Leaf.Core.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Получить следующий случайный элемент.
        /// </summary>
        /// <typeparam name="T">Тип элементов в коллекции</typeparam>
        /// <param name="list">Список с элементами</param>
        /// <returns>Вернет случайный элемент из коллекции.</returns>
        public static T NextRandom<T>(this IReadOnlyList<T> list)
        {
            if (list.Equals(default(IReadOnlyList<T>)) || list.Count == 0)
                return default(T);

            return list[Randomizer.Instance.Next(0, list.Count)];
        }
    }
}