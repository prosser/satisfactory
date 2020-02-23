namespace SatisfactoryTools.Services
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using SatisfactoryTools.Models;
    using SatisfactoryTools.Models.Dto;

    public class PartStore : IPartStore
    {
        private readonly ConcurrentDictionary<int, Part> parts = new ConcurrentDictionary<int, Part>();

        public int Count => this.parts.Count;

        public Part this[int key]
        {
            get => this.parts[key];
            set => this.parts[key] = value;
        }

        public bool Contains(int key)
        {
            return this.parts.ContainsKey(key);
        }

        public Part Get(int id)
        {
            return this.parts[id];
        }

        public IEnumerator<Part> GetEnumerator()
        {
            return this.parts.Values.GetEnumerator();
        }

        public void Load(ItemsDto data)
        {
            int id = 0;
            var fluidIdSet = new HashSet<int>(data.Fluids);
            foreach (string name in data.Parts)
            {
                if (name != "None")
                {
                    Part part = new Part
                    {
                        Id = id,
                        Name = name,
                        PartClass = fluidIdSet.Contains(id) ? PartClass.Fluid : PartClass.Item
                    };

                    if (!this.parts.TryAdd(id, part))
                    {
                        throw new InvalidOperationException($"Duplicate part {part.Name} defined in Items data");
                    }
                }

                ++id;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}