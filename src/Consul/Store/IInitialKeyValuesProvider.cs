namespace PetProjects.Framework.Consul.Store
{
    using System.Collections.Generic;

    public interface IInitialKeyValuesProvider<T>
    {
        IDictionary<string, T> InitialKeyValues { get; }
    }
}