namespace Ohtic.Test.Products.Factories
{
    public static class ConfigurationFactory
    {
        public static IConfiguration Create()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("Properties/launchSettings.json", optional: true, reloadOnChange: true)
                .Build();
        }
    }
}