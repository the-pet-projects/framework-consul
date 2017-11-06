namespace PetProjects.Framework.Consul.Watcher
{
    using System;

    public class WatcherConfiguration : IWatcherConfiguration
    {
        public TimeSpan DelayBetweenFailedRequests { get; set; } = new TimeSpan(0, 0, 0, 60);

        /// <summary>
        /// This timeout must be higher than consul client's timeout.
        /// </summary>
        public TimeSpan BlockingQueryTimeout { get; set; } = new TimeSpan(0, 10, 0);
    }
}
