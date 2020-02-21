namespace SatisfactoryTools.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    internal class ObjectLookupService : IObjectLookupService
    {
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<int, object>> store = new ConcurrentDictionary<Type, ConcurrentDictionary<int, object>>();

        public void Add<T>(int id, T value)
        {
            ConcurrentDictionary<int, object> typedStore = this.store.GetOrAdd(typeof(T), _ => new ConcurrentDictionary<int, object>());
            T added = (T)typedStore.GetOrAdd(id, value);
            if (!Equals(added, value))
            {
                throw new InvalidOperationException($"A different {typeof(T).Name} was already present with id {id}");
            }
        }

        public T Lookup<T>(int id)
        {
            if (this.store.TryGetValue(typeof(T), out ConcurrentDictionary<int, object> typedStore) &&
                typedStore.TryGetValue(id, out object value))
            {
                return (T)value;
            }

            throw new KeyNotFoundException($"Id {id} not tracked for type {typeof(T).Name}");
        }
    }
}