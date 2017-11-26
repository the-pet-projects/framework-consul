namespace PetProjects.Framework.Consul.Store
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using PetProjects.Framework.Consul.Watcher;

    public class ConsulStringKeyValueStore : ConsulKeyValueStore<string>, IStringKeyValueStore
    {
        public ConsulStringKeyValueStore(IStoreConfiguration storeConfig, IKeyValueWatcher watcher, IInitialKeyValuesProvider<string> provider, ILogger log)
            : base(storeConfig, watcher, provider, log)
        {
        }

        protected override Task WatchKeyValueAsync(KeyValuePair<string, string> kv)
        {
            return this.Watcher.WatchStringAsync(kv.Key, kv.Value, updatedValue => this.KeyValueStore.AddOrUpdate(kv.Key, updatedValue, (_, __) => updatedValue));
        }

        public T GetAndConvertValue<T>(string key)
        {
            return (T) ConvertValue(typeof(T), this.Get(key));
        }

        private static bool TryConvertValue(Type type, string value, out object result, out Exception error)
        {
            error = null;
            result = null;
            if (type == typeof(object))
            {
                result = value;
                return true;
            }

            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (string.IsNullOrEmpty(value))
                {
                    return true;
                }
                return TryConvertValue(Nullable.GetUnderlyingType(type), value, out result, out error);
            }

            var converter = TypeDescriptor.GetConverter(type);
            if (converter.CanConvertFrom(typeof(string)))
            {
                try
                {
                    result = converter.ConvertFromInvariantString(value);
                }
                catch (Exception ex)
                {
                    error = new InvalidOperationException(string.Format("Failed to convert '{0}' to type '{1}'.", value, type), ex);
                }
                return true;
            }

            return false;
        }

        private static object ConvertValue(Type type, string value)
        {
            object result;
            Exception error;
            TryConvertValue(type, value, out result, out error);
            if (error != null)
            {
                throw error;
            }
            return result;
        }
    }
}