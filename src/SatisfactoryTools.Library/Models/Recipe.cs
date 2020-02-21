namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SatisfactoryTools.Models.Dto;
    using SatisfactoryTools.Services;

    public class Recipe : NodeTransformer
    {
        public Recipe()
        {
        }

        internal Recipe(IPartStore partStore, int id, RecipeDto dto)
        {
            this.Id = id;
            this.Name = dto.Name;
            this.Time = TimeSpan.FromSeconds(dto.Time);
            this.Inputs = dto.Inputs.Select(x => PartIo.Hydrate(x, partStore)).ToArray();
            this.Outputs = dto.Outputs.Select(x => PartIo.Hydrate(x, partStore)).ToArray();
            this.Builders = dto.Buildings.Select(x => x.Trim().ParseFromDescription<Builder>()).ToHashSet();
            this.IsUnlockable = true;
            this.Building = this.Builders.SingleOrDefault(x => StaticCollections.Buildings.Contains(x));

            // HACK mined items have as their inputs the name of the output!
            this.IsUnlockable = this.Outputs.Any(x => x.Part.Name == this.Name);
        }

        public static Recipe None { get; } = new Recipe { Name = "None" };

        public ISet<Builder> Builders { get; } = new HashSet<Builder>();

        public bool HandBuilt => this.Building == null;

        public int Id { get; }

        public bool IsMined => this.Building switch
        {
            Builder.Miner => true,
            Builder.OilPump => true,
            Builder.WaterExtractor => true,
            _ => false
        };

        public bool IsUnlockable { get; set; }

        public bool IsValid => this.Outputs.Count > 0;

        public string Name { get; set; }

        public bool RequiresBuilding => this.Builders.All(x => x.IsBuilding());

        public TimeSpan Time { get; } = TimeSpan.Zero;

        internal override NodeTransformer Clone(CloneFilters filters)
        {
            throw new NotImplementedException();
        }
    }
}