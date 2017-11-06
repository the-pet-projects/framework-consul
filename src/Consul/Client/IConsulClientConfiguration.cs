namespace PetProjects.Framework.Consul.Client
{
    using System;

    public interface IConsulClientConfiguration
    {
        TimeSpan ClientTimeout { get; }

        string Address { get; }
    }
}