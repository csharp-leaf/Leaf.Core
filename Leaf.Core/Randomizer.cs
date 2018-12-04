using System;

namespace Leaf.Core
{
    /// <summary>
    /// Singleton for Random class.
    /// </summary>
    public static class Randomizer
    {
        [ThreadStatic] private static Random _rand;
        public static Random Instance => _rand ?? (_rand = new Random());
    }
}