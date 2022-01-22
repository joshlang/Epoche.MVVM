namespace Epoche.MVVM.ViewModels.SourceGen;
static class StringExtensions
{
    public static string ToPascalCase(this string s) => string.IsNullOrEmpty(s) ? s : s.Substring(0, 1).ToUpper() + s.Substring(1);
    public static string ToCamelCase(this string s) => string.IsNullOrEmpty(s) ? s : s.Substring(0, 1).ToLower() + s.Substring(1);
    static bool IsInterface(string s) => s?.Length >= 2 && s[0] == 'I' && s.Substring(1, 1) == s.Substring(1, 1).ToUpper();
    public static string WithoutInterface(this string s) => IsInterface(s) ? s.Substring(1) : s;
}
