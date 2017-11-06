namespace PetProjects.Framework.Consul.Store
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PetProjects.Framework.Consul.Watcher;

    public class ConsulStringKeyValueStore : ConsulKeyValueStore<string>
    {
        public ConsulStringKeyValueStore(IStoreConfiguration storeConfig, IKeyValueWatcher watcher, IInitialKeyValuesProvider<string> provider, ILog log)
            : base(storeConfig, watcher, provider, log)
        {
        }

        protected override Task WatchKeyValueAsync(KeyValuePair<string, string> kv)
        {
            return this.Watcher.WatchStringAsync(kv.Key, updatedValue => this.KeyValueStore.AddOrUpdate(kv.Key, updatedValue, (_, __) => updatedValue));
        }
    }
}