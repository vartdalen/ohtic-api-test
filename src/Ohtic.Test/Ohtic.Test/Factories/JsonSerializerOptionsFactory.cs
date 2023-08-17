using System.Text.Json;

namespace Ohtic.Test.Products.Factories
{
	internal static class JsonSerializerOptionsFactory
    {
        internal static JsonSerializerOptions Create()
        {
            return new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
        }
    }
}