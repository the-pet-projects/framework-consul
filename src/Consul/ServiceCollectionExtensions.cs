namespace PetProjects.Framework.Consul
{
    using System;
    using global::Consul;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using PetProjects.Framework.Consul.Client;
    using PetProjects.Framework.Consul.Store;
    using PetProjects.Framework.Consul.Watcher;

    public static class ServiceCollectionExtensions
    {
        public static void AddPetProjectConsulServices(this IServiceCollection collection, IConfiguration configuration, bool createKeys)
        {
            var consulClientConfig = configuration.GetSection("ConsulClientConfiguration");
            var consulWatcherConfig = configuration.GetSection("ConsulWatcherConfiguration");
            var consulStoreConfig = configuration.GetSection("ConsulStoreConfiguration").Get<StoreConfiguration>();

            if (consulWatcherConfig.GetValue<long>("BlockingQueryTimeoutMs") <= 0)
            {
                throw new ArgumentException("BlockingQueryTimeoutMs must be > 0", "BlockingQueryTimeoutMs");
            }

            if (consulWatcherConfig.GetValue<long>("DelayBetweenFailedRequestsMs") <= 0)
            {
                throw new ArgumentException("DelayBetweenFailedRequestsMs must be > 0", "DelayBetweenFailedRequestsMs");
            }

            if (consulClientConfig.GetValue<long>("ClientTimeoutMs") <= 0)
            {
                throw new ArgumentException("ClientTimeoutMs must be > 0", "ClientTimeoutMs");
            }

            collection.AddSingleton<IInitialKeyValuesProvider<string>>(configuration.ToInitialKeyValuesProvider());
            collection.AddSingleton<IWatcherConfiguration>(new WatcherConfiguration
            {
                BlockingQueryTimeout = TimeSpan.FromMilliseconds(consulWatcherConfig.GetValue<long>("BlockingQueryTimeoutMs")),
                DelayBetweenFailedRequests = TimeSpan.FromMilliseconds(consulWatcherConfig.GetValue<long>("DelayBetweenFailedRequestsMs")),
                CreateKeyValues = createKeys
            });
            collection.AddSingleton<IConsulClientConfiguration>(new Client.ConsulClientConfiguration
            {
                Address = consulClientConfig.GetValue<string>("Address"),
                ClientTimeout = TimeSpan.FromMilliseconds(consulClientConfig.GetValue<long>("ClientTimeoutMs"))
            });

            collection.AddSingleton<IStoreConfiguration>(consulStoreConfig);
            collection.AddTransient<IConsulClientFactory, ConsulClientFactory>();
            collection.AddSingleton<IKVEndpoint>(sp => sp.GetRequiredService<IConsulClientFactory>().Create(sp.GetRequiredService<IConsulClientConfiguration>()).KV);
            collection.AddTransient<IKeyValueWatcher, ConsulKeyValueWatcher>();
            collection.AddSingleton<IStringKeyValueStore, ConsulStringKeyValueStore>();
        }
    }
}