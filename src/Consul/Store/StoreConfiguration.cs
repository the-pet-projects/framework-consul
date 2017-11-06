namespace PetProjects.Framework.Consul.Store
{
    public class StoreConfiguration : IStoreConfiguration
    {
        public string Environment { get; set; }

        public string ServiceName { get; set; }

        public string Platform { get; set; }
    }
}