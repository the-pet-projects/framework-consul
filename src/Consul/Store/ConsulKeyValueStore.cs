namespace PetProjects.Framework.Consul.Store
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PetProjects.Framework.Consul.Watcher;

    public class ConsulKeyValueStore<T> : IKeyValueStore<T>
    {
        protected readonly ConcurrentDictionary<string, T> KeyValueStore = new ConcurrentDictionary<string, T>();
        protected readonly IStoreConfiguration StoreConfig;
        protected readonly IKeyValueWatcher Watcher;
        protected readonly IInitialKeyValuesProvider<T> Provider;
        protected readonly ILog Log;

        public ConsulKeyValueStore(IStoreConfiguration storeConfig, IKeyValueWatcher watcher, IInitialKeyValuesProvider<T> provider, ILog log)
        {
            this.StoreConfig = storeConfig;
            this.Watcher = watcher;
            this.Provider = provider;
            this.Log = log;

            this.LoadValuesAndWatchAsync().Wait();
        }

        /// <summary>
        /// Throws KvNotFoundException if key doesn't exist.
        /// </summary>
        /// <exception cref="KvNotFoundException"></exception>
        public T Get(string key)
        {
            key = this.PrependKeyPrefix(key);

            if (this.KeyValueStore.TryGetValue(key, out T value))
            {
                return value;
            }

            throw new KvNotFoundException($"key {key} not found");
        }

        protected virtual Task WatchKeyValueAsync(KeyValuePair<string, T> kv)
        {
            return this.Watcher.WatchAsync<T>(kv.Key, updatedValue => this.KeyValueStore.AddOrUpdate(kv.Key, updatedValue, (_, __) => updatedValue));
        }

        private string PrependKeyPrefix(string key)
        {
            return $"{this.StoreConfig.Environment}/{this.StoreConfig.Platform}/{this.StoreConfig.ServiceName}/{key}";
        }

        private async Task LoadValuesAndWatchAsync()
        {
            foreach (var kv in this.Provider.InitialKeyValues)
            {
                var newKv = new KeyValuePair<string, T>(this.PrependKeyPrefix(kv.Key), kv.Value);
                this.KeyValueStore.AddOrUpdate(newKv.Key, newKv.Value, (_, __) => newKv.Value);
                try
                {
                    await this.WatchKeyValueAsync(newKv).ConfigureAwait(false);
                }
                catch (KvEndpointException ex)
                {
                    this.Log.Error("Key couldn't be loaded from consul and isn't being watched.", () => new { Exception = ex.ToString(), newKv.Key });
                }
            }
        }
    }
}