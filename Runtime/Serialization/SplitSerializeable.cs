using Leaf.Core.Collections.Generic;

namespace Leaf.Core.Runtime.Serialization
{
    internal abstract class SplitSerializeable : IStringSerializeable
    {
        protected const string Splitter = " = ";

        /// <inheritdoc />
        public abstract string SerializeToString();

        /// <inheritdoc />
        public abstract bool DeserializeFromString(string serializedContent);

        /// <inheritdoc />
        public override string ToString() => SerializeToString();
    }
}
