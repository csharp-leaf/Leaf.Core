namespace Leaf.Core.Runtime.Serialization
{
    /// <summary>
    /// Простейшая реализация построковой сериализации и десериализации объектов.
    /// </summary>
    public interface IStringSerializeable
    {
        /// <summary>
        /// Преобразовывает объект в строку, аналогично ToString(), т.е. экспортирует данные в строку.
        /// Не является заменой ToString(), поскольку отвечает только за сериализацию данных.
        /// </summary>
        /// <returns>Возвращается объект в строковом представлении</returns>
        string SerializeToString();

        /// <summary>
        /// Заполняет текущий объект из сериализованных данных, т.е. импортирует данные из строки.
        /// </summary>
        /// <param name="serializedContent">Данные которые нужно импортировать</param>
        /// <returns></returns>
        bool DeserializeFromString(string serializedContent);
    }
}
