using System;

namespace Chsopoly.Libs.Extensions
{
    public static class ActionExtensions
    {
        public static void SafeInvoke (this Action self)
        {
            if (self != null)
            {
                self.Invoke ();
            }
        }

        public static void SafeInvoke<T> (this Action<T> self, T p1)
        {
            if (self != null)
            {
                self.Invoke (p1);
            }
        }

        public static void SafeInvoke<T1, T2> (this Action<T1, T2> self, T1 p1, T2 p2)
        {
            if (self != null)
            {
                self.Invoke (p1, p2);
            }
        }

        public static void SafeInvoke<T1, T2, T3> (this Action<T1, T2, T3> self, T1 p1, T2 p2, T3 p3)
        {
            if (self != null)
            {
                self.Invoke (p1, p2, p3);
            }
        }
    }
}