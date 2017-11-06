namespace PetProjects.Framework.Consul.Store
{
    using System;

    public class KvNotFoundException : Exception
    {
        public KvNotFoundException(string message) : base(message)
        {
        }
    }
}
