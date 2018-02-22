using System.Collections.Generic;

namespace Leaf.Core.Collections.Generic
{
    public class QueueStorage<T> : Queue<T>, IStorage<T>
    {
        /// <inheritdoc />
        public void AppendItem(T item)
        {
            Enqueue(item);
        }

        /// <inheritdoc />
        public T GetNext()
        {
            return Count == 0 ? default(T) : Dequeue();
        }
    }
}
