﻿using System.Collections.Generic;

namespace Actian.EFCore.TestUtilities
{
    public static class CollectionExtensions
    {
        public static Queue<T> Enqueue<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                queue.Enqueue(item);
            }

            return queue;
        }
    }
}
