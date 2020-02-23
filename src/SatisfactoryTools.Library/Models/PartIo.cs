namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Dto;
    using Services;

    [DebuggerDisplay("{Part.Name} @{Rate}/min ({Count})")]
    public class PartIo
    {
        public int Count { get; set; }

        public string Description => $"{this.Part.Name}x{this.Count}{this.Part.Units}";

        public Part Part { get; set; }

        public double Rate { get; set; }

        internal PartIo Clone()
        {
            return new PartIo
            {
                Count = this.Count,
                Part = this.Part,
                Rate = this.Rate
            };
        }


        public static PartIo CreateNone()
        {
            return new PartIo {Part = Part.None, Count = 0, Rate = 0};
        }

        public static PartIo Hydrate(PartIoDto dto, IPartStore store)
        {
            return new PartIo
            {
                Count = dto.Count,
                Part = store.Get(dto.Id),
                Rate = dto.Rate
            };
        }
    }
}