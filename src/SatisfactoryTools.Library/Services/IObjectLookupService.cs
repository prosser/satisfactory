namespace SatisfactoryTools.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;

    public interface IObjectLookupService
    {
        void Add<T>(int id, T value)
            where T : IIdentifiable;

        T Lookup<T>(int id)
            where T : IIdentifiable;

        T Lookup<T>(string name)
            where T : IIdentifiable, INamed;
    }
}