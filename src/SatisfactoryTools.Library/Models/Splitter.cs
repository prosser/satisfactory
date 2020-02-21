namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Splitter : NodeTransformer
    {
        private PartIo input;

        private PartIo[] outputs;

        public int ConnectedOutputs
        {
            get => this.outputs.Length;
            set
            {
                if (value == this.outputs.Length)
                {
                }
            }
        }

        public PartIo Input { get; set; }

        public override IReadOnlyList<PartIo> Inputs
        {
            get => this.input == null ? Array.Empty<PartIo>() : new[] { this.input };
            set
            {
                if (value.Count > 1)
                {
                    throw new NotSupportedException("Cannot assign more than 1 input to a Splitter");
                }

                this.input = value[0];

                if (this.ConnectedOutputs == 0)
                {
                    return;
                }

                double outputRates = this.input.Rate / this.ConnectedOutputs;

                for (int i = 0; i < this.ConnectedOutputs; i++)
                {
                    this.outputs[i].Part = this.input. = new PartIo
                    {
                        Part = this.input.Part,
                        Rate = outputRates,
                        Count = -1
                    };
                }
            }
        }

        public override IReadOnlyList<PartIo> Outputs
        {
            get => this.outputs;
            set => throw new NotSupportedException($"Adjust outputs through {nameof(this.ConnectedOutputs)}");
        }

        internal override NodeTransformer Clone(CloneFilters filters)
        {
            return new Splitter
            {
                Inputs = filters.HasFlag(CloneFilters.Backward) ? this.Inputs.Select(x => x.Clone()).ToArray() : Array.Empty<PartIo>()
            };
        }
    }
}