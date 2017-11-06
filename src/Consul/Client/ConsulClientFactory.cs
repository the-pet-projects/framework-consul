namespace PetProjects.Framework.Consul.Client
{
    using System;
    using global::Consul;

    public class ConsulClientFactory : IConsulClientFactory
    {
        public IConsulClient Create(IConsulClientConfiguration config)
        {
            return new ConsulClient(
                conf =>
                {
                    conf.Address = new Uri(config.Address);
                },
                client =>
                {
                    client.Timeout = config.ClientTimeout;
                });
        }
    }
}