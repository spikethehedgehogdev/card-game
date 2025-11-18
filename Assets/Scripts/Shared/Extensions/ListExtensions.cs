using System;
using System.Collections.Generic;

namespace Shared.Extensions
{
    public static class ListExtensions
    {
        private static readonly Random Rng = new();

        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Rng.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }
    }
}