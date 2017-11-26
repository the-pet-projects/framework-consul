namespace PetProjects.Framework.Consul.Watcher
{
    using System;
    using System.Threading.Tasks;

    public class NullKeyValueWatcher : IKeyValueWatcher
    {
        public void Dispose()
        {
        }

        public Task<IKeyValueWatcher> WatchAsync<T>(string key, T defaultValue, Action<T> updateSettingWith)
        {
            throw new KvEndpointException("Null watcher, possibly error in consul client instantiation");
        }

        public Task<IKeyValueWatcher> WatchStringAsync(string key, string defaultValue, Action<string> updateSettingWith)
        {
            throw new KvEndpointException("Null watcher, possibly error in consul client instantiation");
        }
    }
}