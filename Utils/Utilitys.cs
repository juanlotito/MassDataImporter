namespace importacionmasiva.api.net.Utils
{
    public class Utilitys
    {
        public static IConfigurationSection GetConfigSection(string key)
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);
            IConfigurationRoot root = configurationBuilder.Build();
            return root.GetSection(key);
        }
    }
}
