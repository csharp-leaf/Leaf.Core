using System.Collections.Generic;

namespace Leaf.Core.Collections.Generic
{
    public sealed class ListStorage<T> : List<T>, IStorage<T>
    {
        public ListIteration Iteration = ListIteration.TillTheEnd;
        private int _currentIndex;

        /// <summary>
        /// Сбрасывает текущий элемент списка на первый (нулевой).
        /// </summary>
        public void ResetPointer()
        {
            _currentIndex = 0;
        }

        void IStorage<T>.AppendItem(T item)
        {
            Add(item);
        }
        
        T IStorage<T>.GetNext()
        {
            if (Count == 0)
                return default(T);

            if (_currentIndex >= Count)
            {
                if (Iteration == ListIteration.TillTheEnd)
                    return default(T);

                _currentIndex = 0;
            }

            var result = this[_currentIndex];
            if (Iteration == ListIteration.Removable)
            {
                RemoveAt(_currentIndex);
                // Don't increment index!
            }
            else
            {
                ++_currentIndex;
            }

            return result;
        }
    }
}
