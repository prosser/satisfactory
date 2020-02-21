namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class NodeTransformer
    {
        public Builder? Building { get; set; }

        public virtual IReadOnlyList<PartIo> Inputs { get; set; } = Array.Empty<PartIo>();

        public virtual IReadOnlyList<PartIo> Outputs { get; set; } = Array.Empty<PartIo>();

        internal abstract NodeTransformer Clone(CloneFilters filters);
    }
}