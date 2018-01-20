using System.Collections.Generic;

namespace Leaf.Core.Collections.Generic
{
    public class MaterialsStorageQueue<T> : Queue<T>, IMaterialsStorage<T>
    {
        /// <inheritdoc />
        void IMaterialsStorage<T>.AppendItem(T item)
        {
            Enqueue(item);
        }

        /// <inheritdoc />
        public T GetNext()
        {
            return Dequeue();
        }
    }
}
