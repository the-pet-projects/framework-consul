namespace PetProjects.Framework.Consul.Store
{
    public interface IStringKeyValueStore : IKeyValueStore<string>
    {
        T GetAndConvertValue<T>(string key);
    }
}