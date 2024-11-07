namespace Labb_BlazorApp.Extensions;

public static class StringExtensions
{
    public static string OrIfEmpty(this string? str, string defaultValue)
    {
        return !string.IsNullOrEmpty(str) ? str : defaultValue;
    }

    public static string OrIfEmpty(this string? self, Func<string> defaultValue)
    {
        return !string.IsNullOrEmpty(self) ? self : defaultValue();
    }
}