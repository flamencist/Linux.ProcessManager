using System;
using System.Collections.Generic;

namespace Linux
{
    internal static class EnumerableExtensions
    {
        internal static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var e in enumerable)
            {
                action(e);
            }
        }
    }
}