using Leaf.Core.Collections.Generic;

namespace Leaf.Core.Collections
{
    /// <summary>
    /// Определяет тип перечисления списка.
    /// </summary>
    public enum ListIteration
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
    /// Потокобезопасный список строк.
    /// </summary>
    public class LockedList : LockedList<string>
    {
        ///// <inheritdoc />
        //public LockedList(IEnumerable<string> items) : base(items)
        //{
        //}      
    }
}
