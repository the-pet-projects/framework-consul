namespace PetProjects.Framework.Consul.Store
{
    public interface IKeyValueStore<out T>
    {
        T Get(string key);
    }
}