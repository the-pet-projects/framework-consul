namespace PetProjects.Framework.Consul.Watcher
{
    using System;
    using System.Threading.Tasks;

    public interface IKeyValueWatcher : IDisposable
    {
        Task<IKeyValueWatcher> WatchAsync<T>(string key, Action<T> updateSettingWith);

        Task<IKeyValueWatcher> WatchStringAsync(string key, Action<string> updateSettingWith);
    }
}