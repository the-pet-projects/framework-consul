namespace PetProjects.Framework.Consul.Watcher
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Consul;
    using PetProjects.Framework.Consul;

    public class ConsulKeyValueWatcher : IKeyValueWatcher
    {
        private readonly IKVEndpoint keyValueEndpoint;
        private readonly IWatcherConfiguration configuration;
        private readonly ILog log;

        private readonly CancellationTokenSource ct = new CancellationTokenSource();
        private readonly ConcurrentBag<Task> tasks = new ConcurrentBag<Task>();

        public ConsulKeyValueWatcher(IKVEndpoint keyValueEndpoint) : this(keyValueEndpoint, new WatcherConfiguration(), new NullLog())
        {
        }

        public ConsulKeyValueWatcher(IKVEndpoint keyValueEndpoint, ILog log) : this(keyValueEndpoint, new WatcherConfiguration(), log)
        {
        }

        public ConsulKeyValueWatcher(IKVEndpoint keyValueEndpoint, IWatcherConfiguration configuration) : this(keyValueEndpoint, configuration, new NullLog())
        {
        }

        public ConsulKeyValueWatcher(IKVEndpoint keyValueEndpoint, IWatcherConfiguration configuration, ILog log)
        {
            this.keyValueEndpoint = keyValueEndpoint;
            this.configuration = configuration;
            this.log = log;
        }

        public async Task<IKeyValueWatcher> WatchAsync<T>(string key, Action<T> updateSettingWith)
        {
            var lastIndex = await this.SetInitialValueAsync(key, result => updateSettingWith(result.GetDeserializedValue<T>())).ConfigureAwait(false);
            this.StartWatching(key, lastIndex, result => updateSettingWith(result.GetDeserializedValue<T>()));

            return this;
        }

        public async Task<IKeyValueWatcher> WatchStringAsync(string key, Action<string> updateSettingWith)
        {
            var lastIndex = await this.SetInitialValueAsync(key, result => updateSettingWith(result.GetString())).ConfigureAwait(false);
            this.StartWatching(key, lastIndex, result => updateSettingWith(result.GetString()));

            return this;
        }

        public void Dispose()
        {
            this.log.Warning("Stopping consul key value watch tasks...");

            this.ct.Cancel();

            foreach (var task in this.tasks)
            {
                task.Wait();
            }

            this.ct.Dispose();

            this.log.Warning("Consul key value watcher is stopped.");
        }

        private async Task<ulong> SetInitialValueAsync(string key, Action<QueryResult<KVPair>> updateSettingWith)
        {
            try
            {
                var result = await this.keyValueEndpoint.Get(key, new QueryOptions()).ConfigureAwait(false);

                if (result.Response != null)
                {
                    this.log.Info("Successfuly retrieved initial value from consul.", () => new { Key = key, Value = result.GetString() });
                    updateSettingWith(result);
                }
                else
                {
                    this.log.Warning("Key doesn't exist in consul. Action will be executed when the key is created.", () => new { Key = key });
                }

                return result.LastIndex;
            }
            catch (Exception ex)
            {
                throw new KvEndpointException($"Couldn't load key '{key}' from consul", ex);
            }
        }

        private void StartWatching(string key, ulong lastIndex, Action<QueryResult<KVPair>> updateSettingWith)
        {
            this.tasks.Add(Task.Factory.StartNew(
                async () =>
                {
                    while (true)
                    {
                        try
                        {
                            if (this.ct.IsCancellationRequested)
                            {
                                break;
                            }

                            this.log.Info(
                                "Executing blocking query to kv get endpoint.",
                                () => new { Key = key, TimeoutMilliseconds = this.configuration.BlockingQueryTimeout.TotalMilliseconds });

                            var result = await this.keyValueEndpoint.Get(
                                key,
                                new QueryOptions { WaitIndex = lastIndex, WaitTime = this.configuration.BlockingQueryTimeout },
                                this.ct.Token).ConfigureAwait(false);

                            if (result.LastIndex != lastIndex && !this.ct.IsCancellationRequested)
                            {
                                this.log.Info("A new value was returned.", () => new { Key = key, Value = result.GetString() });

                                updateSettingWith(result);

                                lastIndex = result.LastIndex;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (this.ct.IsCancellationRequested)
                            {
                                break;
                            }

                            this.log.Error(
                                "Blocking query to kv get endpoint failed. Waiting until next query.",
                                () => new
                                {
                                    Exception = ex.ToString(),
                                    DelayMilliseconds = this.configuration.DelayBetweenFailedRequests.TotalMilliseconds
                                });

                            await Task.Delay(this.configuration.DelayBetweenFailedRequests, this.ct.Token);
                        }
                    }
                },
                TaskCreationOptions.LongRunning));
        }
    }
}