using System;

namespace Chsopoly.Libs.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsStruct (this Type type)
        {
            return type.IsValueType && !type.IsPrimitive;
        }
    }
}