using System.Collections.Generic;

namespace Chsopoly.Libs.Extensions
{
    public static class ListExtensions
    {
        public static void AddIfNotNull<T> (this List<T> self, T element)
        {
            if (element != null)
            {
                self.Add (element);
            }
        }
    }
}