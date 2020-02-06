using System.Text.RegularExpressions;

namespace Chsopoly.Libs.Extensions
{
    public static class StringExtensions
    {
        public static string Indent (this string str, int spaces)
        {
            str = str.Trim ().ReplaceEOL ("\n");
            str = Regex.Replace (str, @"^", "".PadLeft (spaces), RegexOptions.Multiline);
            return str;
        }

        public static string ReplaceEOL (this string str, string newValue)
        {
            return str.Replace ("\r\n", newValue).Replace ("\r", newValue).Replace ("\n", newValue);
        }

        public static string RemoveLast (this string str)
        {
            if (str.Length > 0)
            {
                return str.Remove (str.Length - 1);
            }
            return str;
        }

        public static string RemoveLast (this string str, int count)
        {
            if (str.Length > 0)
            {
                return str.Remove (str.Length - count, count);
            }
            return str;
        }
    }
}