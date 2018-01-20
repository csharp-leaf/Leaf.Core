using System;
using Leaf.Core.Collections.Generic;

namespace Leaf.Core.Collections
{
    public static class CollectionRandimizer
    {
        [ThreadStatic] private static Random _rand;
        private static Random Rand => _rand ?? (_rand = new Random());

        /// <summary>
        /// Получает случайный элемент коллекции
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        /// 
        /// HACK: Решение для Genetic Random.
        public static T GetNextRandom<T>(this MaterialsStorageList<T> collection)
        {
            if (collection.Count == 0)
                return default(T);

            int index = Rand.Next(collection.Count - 1);
            var result = collection[index];

            if (collection.Iteration == MaterialsListIteration.Removable)
                collection.RemoveAt(index);

            return result;
        }
    }
}
