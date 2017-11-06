namespace PetProjects.Framework.Consul.Client
{
    using System;

    public class ConsulClientConfiguration : IConsulClientConfiguration
    {
        public TimeSpan ClientTimeout { get; set; } = new TimeSpan(0, 15, 0);

        public string Address { get; set; }
    }
}
