namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class NodeTransformer
    {
        public Builder? Building { get; set; }

        public IReadOnlyList<PartIo> Inputs { get; private set; } = Array.Empty<PartIo>();

        public IReadOnlyList<PartIo> Outputs { get; private set; } = Array.Empty<PartIo>();

        internal abstract NodeTransformer Clone(CloneFilters filters);

        protected void SetInputs(IReadOnlyList<PartIo> inputs)
        {
            this.Inputs = inputs;
        }

        protected void SetOutputs(IReadOnlyList<PartIo> outputs)
        {
            this.Outputs = outputs;
        }
    }
}