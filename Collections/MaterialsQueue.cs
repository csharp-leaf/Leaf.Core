using System.Text;
using Leaf.Core.Collections.Generic;

namespace Leaf.Core.Collections
{
    /// <summary>
    /// Потокобезопасная очередь строк-материалов.
    /// </summary>
    public class MaterialsQueue : MaterialsQueue<string>
    {
        // /// <inheritdoc />
        //public MaterialsQueue(IEnumerable<string> items) : base(items)
        //{
        // }

        /// <summary>
        /// Поочередно извлекает все материалы из очереди и записывает в строку.
        /// </summary>
        /// <returns>Строка со всеми материалами. Каждый материал будет идти с новой строки.</returns>
        public string DeqeueAllToString()
        {
            if (MaterialsStorage == null)
                return string.Empty;

            var sb = new StringBuilder();

            lock (MaterialsStorage)
            {
                int materialsCount = MaterialsStorage.Count;

                for (int i = 0; i < materialsCount; i++)
                {
                    string material = MaterialsStorage.GetNext();
                    if (!string.IsNullOrEmpty(material))
                        sb.AppendLine(material.Trim());
                }
            }

            return sb.ToString();
        }
    }
}
