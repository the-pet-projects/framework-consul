namespace PetProjects.Framework.Consul.Store
{
    public interface IStoreConfiguration
    {
        string Environment { get; }

        string ServiceName { get; }

        string Platform { get; }
    }
}