namespace PetProjects.Framework.Consul.Watcher
{
    using System;

    public interface IWatcherConfiguration
    {
        TimeSpan DelayBetweenFailedRequests { get; }

        /// <summary>
        /// This timeout must be higher than consul client's timeout.
        /// </summary>
        TimeSpan BlockingQueryTimeout { get; }

        bool CreateKeyValues { get; }
    }
}
