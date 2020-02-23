namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using SatisfactoryTools.Models.Dto;
    using SatisfactoryTools.Services;

    [DebuggerDisplay("{Part.Name} @{Rate}/min ({Count})")]
    public class PartIo
    {
        public int Count { get; set; }

        public Part Part { get; set; }

        public double Rate { get; set; }

        public static PartIo Hydrate(PartIoDto dto, IPartStore store)
        {
            return new PartIo
            {
                Count = dto.Count,
                Part = store.Get(dto.Id),
                Rate = dto.Rate
            };
        }

        internal PartIo Clone()
        {
            return new PartIo
            {
                Count = this.Count,
                Part = this.Part,
                Rate = this.Rate
            };
        }
    }
}