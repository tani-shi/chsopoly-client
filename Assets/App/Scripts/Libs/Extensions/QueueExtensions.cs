using System.Collections.Generic;
using UnityEngine;

namespace Chsopoly.Libs.Extensions
{
    public static class QueueExtensions
    {
        public static void Remove<T> (this Queue<T> queue, T remove) where T : class
        {
            if (!queue.Contains (remove))
            {
                return;
            }
            for (int i = 0; i < queue.Count; i++)
            {
                var item = queue.Dequeue ();
                if (item != remove)
                {
                    queue.Enqueue (item);
                }
            }
        }
    }
}