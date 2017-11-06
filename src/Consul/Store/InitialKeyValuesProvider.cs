namespace PetProjects.Framework.Consul.Store
{
    using System.Collections.Generic;

    public class InitialKeyValuesProvider<T> : IInitialKeyValuesProvider<T>
    {
        public IDictionary<string, T> InitialKeyValues { get; set; }
    }
}