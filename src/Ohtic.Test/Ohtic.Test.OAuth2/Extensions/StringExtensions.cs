namespace Ohtic.Test.OAuth.Extensions
{
    internal static class StringExtensions
    {
        internal static bool IsGoogle(this string input)
        {
            return input.ToLower() == "Google".ToLower();
        }

        internal static bool IsMicrosoft(this string input)
        {
            return input.ToLower() == "Microsoft".ToLower();
        }
    }
}