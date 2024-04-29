using System.Text.RegularExpressions;

namespace MikietaApi.Helpers;

public static class RegexHelper
{
    public static bool IsMatch(string pattern, string value)
    {
        var regex = new Regex(pattern);

        return regex.IsMatch(value);
    }
}