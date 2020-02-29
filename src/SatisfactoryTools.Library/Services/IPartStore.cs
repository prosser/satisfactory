namespace SatisfactoryTools.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using Models.Dto;

    public interface IPartStore : IEnumerable<Part>
    {
        int Count { get; }

        Part Get(int id);

        void Load(ItemsDto data);
    }
}