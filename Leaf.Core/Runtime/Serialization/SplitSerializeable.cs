// ReSharper disable UnusedMember.Global

using System;

namespace Leaf.Core.Runtime.Serialization
{
    /// <inheritdoc />
    [Serializable]
    public abstract class SplitSerializeable : IStringSerializeable
    {
        /// <summary>
        /// Разделитель между значениями. По умолчанию равен ":".
        /// </summary>
        protected virtual string Splitter { get; } = ":";

        /// <inheritdoc />
        public abstract string SerializeToString();

        /// <inheritdoc />
        public abstract bool DeserializeFromString(string serializedContent);

        /// <inheritdoc />
        public override string ToString() => SerializeToString();
    }
}
