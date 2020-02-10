using System;

namespace Chsopoly.Libs.Extensions
{
    public static class FloatExtensions
    {
        public static float Half (this Single self)
        {
            return self * 0.5f;
        }
    }
}