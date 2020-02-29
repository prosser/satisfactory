namespace SatisfactoryTools.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Models;

    internal class ObjectLookupService : IObjectLookupService
    {
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, int>> idMap =
            new ConcurrentDictionary<Type, ConcurrentDictionary<string, int>>();

        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<int, IIdentifiable>> store =
            new ConcurrentDictionary<Type, ConcurrentDictionary<int, IIdentifiable>>();

        public void Add<T>(int id, T value)
            where T : IIdentifiable
        {
            ConcurrentDictionary<int, IIdentifiable> typedStore =
                this.store.GetOrAdd(typeof(T), _ => new ConcurrentDictionary<int, IIdentifiable>());
            var added = (T)typedStore.GetOrAdd(id, value);

            if (!Equals(added, value))
            {
                throw new InvalidOperationException($"A different {typeof(T).Name} was already present with id {id}");
            }

            if (value is INamed named)
            {
                ConcurrentDictionary<string, int> typedMap =
                    this.idMap.GetOrAdd(typeof(T), _ => new ConcurrentDictionary<string, int>());
                int addedId = typedMap.GetOrAdd(named.Name, id);

                if (id != addedId)
                {
                    throw new InvalidOperationException(
                        $"A different {typeof(T).Name} was already present with name {named.Name}");
                }
            }
        }

        public T Lookup<T>(string name)
            where T : IIdentifiable, INamed
        {
            if (this.idMap.TryGetValue(typeof(T), out ConcurrentDictionary<string, int> typedMap) &&
                typedMap.TryGetValue(name, out int id))
            {
                return this.Lookup<T>(id);
            }

            throw new KeyNotFoundException($"Name {name} not tracked for type {typeof(T).Name}");
        }

        public T Lookup<T>(int id)
            where T : IIdentifiable
        {
            if (this.store.TryGetValue(typeof(T), out ConcurrentDictionary<int, IIdentifiable> typedStore) &&
                typedStore.TryGetValue(id, out IIdentifiable value))
            {
                return (T)value;
            }

            throw new KeyNotFoundException($"Id {id} not tracked for type {typeof(T).Name}");
        }
    }
}