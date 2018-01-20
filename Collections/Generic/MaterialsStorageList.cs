using System.Collections.Generic;

namespace Leaf.Core.Collections.Generic
{
    public class MaterialsStorageList<T> : List<T>, IMaterialsStorage<T>
    {
        public MaterialsListIteration Iteration = MaterialsListIteration.TillTheEnd;
        private int _currentIndex;

        /// <summary>
        /// Сбрасывает текущий элемент списка на первый (нулевой).
        /// </summary>
        public void ResetPointer()
        {
            _currentIndex = 0;
        }

        /// <inheritdoc />
        void IMaterialsStorage<T>.AppendItem(T item)
        {
            Add(item);
        }
        
        /// <inheritdoc />
        T IMaterialsStorage<T>.GetNext()
        {
            if (Count == 0)
                return default(T);

            if (_currentIndex >= Count)
            {
                if (Iteration == MaterialsListIteration.TillTheEnd)
                    return default(T);

                _currentIndex = 0;
            }

            var result = this[_currentIndex];
            if (Iteration == MaterialsListIteration.Removable)
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
