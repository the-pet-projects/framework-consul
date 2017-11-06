namespace PetProjects.Framework.Consul.Watcher
{
    using System.Text;
    using global::Consul;
    using Newtonsoft.Json;

    internal static class ConsulResponseExtensions
    {
        internal static T GetDeserializedValue<T>(this QueryResult<KVPair> result)
        {
            return JsonConvert.DeserializeObject<T>(result.GetString());
        }

        internal static string GetString(this QueryResult<KVPair> result)
        {
            return Encoding.UTF8.GetString(result.Response.Value, 0, result.Response.Value.Length);
        }
    }
}
