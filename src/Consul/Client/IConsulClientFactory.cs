namespace PetProjects.Framework.Consul.Client
{
    using global::Consul;

    public interface IConsulClientFactory
    {
        IConsulClient Create(IConsulClientConfiguration config);
    }
}