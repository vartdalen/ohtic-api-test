using System.Text.RegularExpressions;

namespace Ohtic.Test.Api.Extensions
{
    internal static class StringExtensions
    {
        internal static string FromPascalCaseToPascalCaseWithSpaces(this string input)
        {
            return Regex.Replace(input, @"(?<!^)(\p{Lu})", " $1");
        }
    }
}