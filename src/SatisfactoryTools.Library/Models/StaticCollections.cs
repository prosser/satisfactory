namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class StaticCollections
    {
        public static ISet<Builder> Buildings { get; } = new HashSet<Builder>(
            new[]
            {
                Builder.Constructor,
                Builder.Assembler,
                Builder.Manufacturer,
                Builder.WaterExtractor,
                Builder.OilPump,
                Builder.Miner,
                Builder.OilRefinery,
                Builder.Foundry,
                Builder.Smelter
            });
    }
}