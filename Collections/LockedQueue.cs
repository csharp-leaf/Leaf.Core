using System.Collections.Generic;
using System.Text;
using Leaf.Core.Collections.Generic;

namespace Leaf.Core.Collections
{
    /// <summary>
    /// Потокобезопасная очередь строк.
    /// </summary>
    public class LockedQueue : LockedQueue<string>
    {
        public LockedQueue() {}

        public LockedQueue(IEnumerable<string> items) : base(items) {}

        /// <summary>
        /// Поочередно извлекает все элементы из очереди и записывает каждый в общую строку.
        /// </summary>
        /// <returns>Строка со всеми элементами. Каждый элементами будет идти с новой строки.</returns>
        public string DeqeueAllToString()
        {
            if (Storage == null)
                return string.Empty;

            var sb = new StringBuilder();

            lock (Storage)
            {
                int materialsCount = Storage.Count;

                for (int i = 0; i < materialsCount; i++)
                {
                    string material = Storage.GetNext();
                    if (!string.IsNullOrEmpty(material))
                        sb.AppendLine(material.Trim());
                }
            }

            return sb.ToString();
        }
    }
}
