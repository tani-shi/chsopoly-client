using UnityEngine;

namespace Chsopoly.Libs.Extensions
{
    public static class UnityObjectExtensions
    {
        public static T CreateInstance<T> (this T original, Transform parent = null) where T : Object
        {
            T obj;
            if (parent == null)
            {
                obj = Object.Instantiate<T> (original);
            }
            else
            {
                obj = Object.Instantiate<T> (original, parent);
            }
            if (obj != null)
            {
                obj.name = obj.name.Replace ("(Clone)", "");
            }
            return obj;
        }
    }
}