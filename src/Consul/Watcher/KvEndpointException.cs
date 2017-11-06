namespace PetProjects.Framework.Consul.Watcher
{
    using System;

    public class KvEndpointException : Exception
    {
        public KvEndpointException(string message) : base(message)
        {
        }

        public KvEndpointException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}