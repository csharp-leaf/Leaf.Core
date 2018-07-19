using System.Collections.Generic;

namespace Leaf.Core.Collections.Generic
{
    public class QueueStorage<T> : Queue<T>, IStorage<T>
    {
        public void AppendItem(T item)
        {
            Enqueue(item);
        }

        public T GetNext()
        {
            return Count == 0 ? default(T) : Dequeue();
        }
    }
}
