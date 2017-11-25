namespace PetProjects.Framework.Consul
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;
    using PetProjects.Framework.Consul.Store;

    public static class ConfigurationExtensions
    {
        public static IInitialKeyValuesProvider<string> ToInitialKeyValuesProvider(this IConfiguration config)
        {
            var dic = new Dictionary<string, string>();

            config.ProcessConfiguration(dic, string.Empty);

            return new InitialKeyValuesProvider<string>
            {
                InitialKeyValues = dic
            };
        }

        private static void ProcessConfiguration(this IConfiguration config, IDictionary<string, string> dic, string key)
        {
            foreach (var configSection in config.GetChildren())
            {
                configSection.ProcessChildSection(dic, key);
            }
        }

        private static void ProcessChildSection(this IConfigurationSection configSection, IDictionary<string, string> dic, string key)
        {
            if (!configSection.Exists())
            {
                return;
            }

            var newKey = key != string.Empty ? key + "/" + configSection.Key : configSection.Key;

            if (configSection.Value != null)
            {
                dic.Add(newKey, configSection.Value);
            }

            configSection.ProcessConfiguration(dic, newKey);
        }
    }
}