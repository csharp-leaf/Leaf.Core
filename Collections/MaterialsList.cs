using System.Collections.Generic;
using Leaf.Core.Collections.Generic;

namespace Leaf.Core.Collections
{
    /// <summary>
    /// Определяет тип перечисления списка.
    /// </summary>
    public enum MaterialsListIteration
    {
        /// <summary>
        /// Список будет перечисляться пока не достигнет конца. Элементы не удаляются.
        /// </summary>
        TillTheEnd,
        /// <summary>
        /// Список будет перечисляться бесконечно - после последнего элемента следует первый.
        /// </summary>
        Looped,
        /// <summary>
        /// Список будет перечисляться пока не достигнет конца. Перечисленные элементы удаляются.
        /// </summary>
        Removable
    }

    /// <summary>
    /// Потокобезопасный список строк-материалов.
    /// </summary>
    public class MaterialsList : MaterialsList<string>
    {
        /// <inheritdoc />
        public MaterialsList(IEnumerable<string> items) : base(items)
        {
        }
    }
}
