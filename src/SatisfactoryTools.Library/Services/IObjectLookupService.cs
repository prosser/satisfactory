namespace SatisfactoryTools.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IObjectLookupService
    {
        void Add<T>(int id, T value);

        T Lookup<T>(int id);
    }
}