namespace Leaf.Core.Runtime.Serialization
{ 
    public abstract class SplitSerializeable : IStringSerializeable
    {
        /// <summary>
        /// Разделитель между значениями. По умолчанию равен " = ".
        /// </summary>
        protected virtual string Splitter { get; } = " = ";

        public abstract string SerializeToString();

        public abstract bool DeserializeFromString(string serializedContent);

        public override string ToString() => SerializeToString();
    }
}
