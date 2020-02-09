using System.Security.Cryptography;
using System.Text;
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

        public static string ToSha256Hash (this string str)
        {
            using (var sha256 = SHA256.Create ())
            {
                var bytes = sha256.ComputeHash (Encoding.UTF8.GetBytes (str));
                var builder = new StringBuilder ();
                foreach (var b in bytes)
                {
                    builder.Append (b.ToString ("x2"));
                }
                return builder.ToString ();
            }
        }
    }
}