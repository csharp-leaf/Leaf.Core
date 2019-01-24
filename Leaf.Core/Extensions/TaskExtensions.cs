using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Leaf.Core.Extensions
{
    public static class TaskExtensions
    {
        public static ConfiguredTaskAwaitable Caf(this Task task) => task.ConfigureAwait(false);
        public static ConfiguredTaskAwaitable<T> Caf<T>(this Task<T> task) => task.ConfigureAwait(false);
    }
}