using System.Text.RegularExpressions;

namespace SmartQQ
{
    public static class SmartQQExtension
    {
        public static bool IsMatch(this string source, string pattern, RegexOptions options = RegexOptions.IgnoreCase)
        {
            Regex regex = new Regex(pattern, options);
            return regex.IsMatch(source);
        }
    }
}
