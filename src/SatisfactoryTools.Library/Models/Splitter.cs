namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Splitter : NodeTransformer
    {
        private PartIo input;

        private readonly List<PartIo> outputs = new List<PartIo>();

        public int ConnectedOutputs
        {
            get => this.outputs.Count;
            set
            {
                if (value == this.outputs.Count)
                {
                    return;
                }

                if (value > this.outputs.Count)
                {
                    this.outputs.Add(new PartIo());
                }
                else
                {
                    this.outputs.RemoveAt(this.outputs.Count - 1);
                }

                this.ConfigureOutputs();
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

                this.ConfigureOutputs();
            }
        }

        private void ConfigureOutputs()
        {
            if (this.ConnectedOutputs == 0)
            {
                return;
            }

            double rate = this.input.Rate / this.ConnectedOutputs;

            for (int i = 0; i < this.ConnectedOutputs; i++)
            {
                this.outputs[i].Part = this.input.Part;
                this.outputs[i].Rate = rate;
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